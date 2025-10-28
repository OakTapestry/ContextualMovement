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

    public List<GameObject> targets = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = Random.Range(3, 8);
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.transform.position = new Vector3(0,-20,0);
        }
        if (grounded)
        {
            // Rotate the mover if an object is detected in front
            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
            {
                transform.LookAt(transform.position - hitFront.normal);

                RotateAway();
            }
            //move towards or away from target if there is one
            else if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (!Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), targets[i].transform.position - transform.position, Quaternion.identity, Vector3.Distance(transform.position, targets[i].transform.position)))
                    {
                        if (isHunter == false)
                        {
                            bool foundHunter = false;
                            for (int j = 0; j < targets.Count; j++)
                            {
                                if (targets[j].CompareTag("Hunter"))
                                {
                                    transform.LookAt(targets[j].transform.position);
                                    transform.Rotate(Vector3.up, 180);
                                    foundHunter = true;
                                    j = targets.Count; // Exit loop
                                }
                            }
                            if (foundHunter == false)
                            {
                                transform.LookAt(targets[0].transform.position);
                            }
                        }
                        else
                        {
                            bool foundRunner = false;
                            for (int j = 0; j < targets.Count; j++)
                            {
                                if (targets[j].CompareTag("Runner"))
                                {
                                    transform.LookAt(targets[j].transform.position);
                                    foundRunner = true;
                                    j = targets.Count; // Exit loop
                                }
                            }
                            if (foundRunner == false)
                            {
                                transform.LookAt(targets[0].transform.position);
                            }
                        }


                    }
                }
                

                for (int i = 0; i < targets.Count; i++)
                {
                    if (isHunter)
                    {
                        if (Vector3.Distance(transform.position, targets[i].transform.position) < 0.5f && targets[i].CompareTag("Hunter") == false)
                        {
                            targets[i].SetActive(false);
                        }

                        if (targets[i].activeSelf == false)
                        {
                            targets.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, targets[i].transform.position) < 0.5f && targets[i].CompareTag("Hunter") == false && targets[i].CompareTag("Runner") == false)
                        {
                            targets[i].SetActive(false);
                        }

                        if (targets[i].activeSelf == false)
                        {
                            targets.RemoveAt(i);
                        }
                    }
                    
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
        if (collision.collider.CompareTag("Ground"))
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
        if (other.CompareTag("Hunter") || other.CompareTag("Runner"))
        {
            targets.Add(other.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
        if (other.CompareTag("Hunter") || other.CompareTag("Runner"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
    }
}
