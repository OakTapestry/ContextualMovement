using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AdvancedMover : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    RaycastHit hitFront, hitLeft, hitRight;
    [SerializeField] float forwardDist, sideDist, downDist;
    bool leftWall, rightWall;

    int randInt;

    [SerializeField] GameObject downCheck, jumpCheck;
    [SerializeField] bool isHunter, looksLikeRunner;

    Rigidbody rbody;

    bool grounded;

    public List<GameObject> targets = new List<GameObject>(), runners = new(), hunters = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = Random.Range(3, 8);
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (grounded)
        {
            // Rotate the mover if an object is detected in front
            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
            {
                transform.LookAt(transform.position - hitFront.normal);

                RotateAway();
            }
            else if (!isHunter && hunters.Count > 0)
            {
                //draw line straight to hunter to see if there is a wall in between
                Physics.Linecast(transform.position + new Vector3(0, 0.1f, 0), hunters[0].transform.position + new Vector3(0, 0.1f, 0), out RaycastHit hit);
                if (hit.collider == null || !hit.collider.gameObject.CompareTag("Wall"))
                {
                    //no wall in between, do boxcast to see if the hunter can even reach the runner through the potential wall gap
                    Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), hunters[0].transform.position - transform.position, out hit, Quaternion.identity, Vector3.Distance(transform.position, hunters[0].transform.position));

                    //the hunter can reach the runner, so run away
                    if (hit.collider.gameObject.CompareTag("Hunter") || hit.collider == null)
                    {
                        transform.LookAt(hunters[0].transform.position);
                        transform.Rotate(Vector3.up, 180);
                    }
                }

            }
            else if (isHunter && runners.Count > 0)
            {
                Physics.Linecast(transform.position + new Vector3(0, 0.1f, 0), runners[0].transform.position + new Vector3(0, 0.1f, 0), out RaycastHit hit);


                if (hit.collider == null || !hit.collider.gameObject.CompareTag("Wall"))
                {
                    Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), runners[0].transform.position - transform.position, out hit, Quaternion.identity, Vector3.Distance(transform.position, runners[0].transform.position));
                    if (hit.collider.gameObject.CompareTag("Runner") || hit.collider == null)
                    {
                        transform.LookAt(runners[0].transform.position);
                    }
                }
            }
            else if (targets.Count > 0)
            {
                if (!Physics.Linecast(transform.position + new Vector3(0, 0.1f, 0), targets[0].transform.position + new Vector3(0, 0.1f, 0)))

                    if (!Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), targets[0].transform.position - transform.position, Quaternion.identity, Vector3.Distance(transform.position, targets[0].transform.position)))
                    {
                        transform.LookAt(targets[0].transform.position);
                    }

            }

            if (targets.Count > 0)
            {
                if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.5f)
                {
                    targets[0].SetActive(false);
                }

                if (targets[0].activeSelf == false)
                {
                    targets.RemoveAt(0);
                }
            }

            // Rotate the mover if a hole is detected in front
            if (!Physics.BoxCast(downCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
            {
                // Check if there is a floor to jump to
                if (Physics.BoxCast(jumpCheck.transform.position, new Vector3(0.3f, 0.9f, 0.3f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
                {
                    // Check to make sure there is no object in the way of the jump
                    if (!Physics.CheckBox(jumpCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f)))
                    {
                        rbody.AddRelativeForce(transform.up * 300);
                        grounded = false;
                    }
                    else
                    {
                        RotateAway();
                    }
                }
                else
                {
                    RotateAway();
                }
            }
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.fixedDeltaTime);
    }

    void RotateAway()
    {
        leftWall = false;
        rightWall = false;

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), -transform.right, out hitLeft, Quaternion.identity, sideDist))
        {
            leftWall = true;
        }

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), transform.right, out hitRight, Quaternion.identity, sideDist))
        {
            rightWall = true;
        }

        if (leftWall && !rightWall)
        {
            transform.Rotate(Vector3.up, 90);
        }
        else if (!leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, -90);
        }
        else if (leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, 180);
        }
        else
        {
            randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                transform.Rotate(Vector3.up, 90);
            }
            else
            {
                transform.Rotate(Vector3.up, -90);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            targets.Add(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Runner"))
        {
            runners.Add(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Hunter"))
        {
            hunters.Add(other.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Runner"))
        {
            runners.Remove(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Hunter"))
        {
            hunters.Remove(other.transform.parent.gameObject);
        }
    }
}