using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
    bool speedBoosted;
    float speedBoostTimer;

    bool disguiseActive;
    bool disguiseAlmostActive;
    float disguiseTimer;

    bool deactivateWayOutSearch = false;

    bool runningAway = false;
    float runAwayTimer = 0;

    public List<GameObject> targets = new List<GameObject>(), runners = new(), hunters = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isHunter)
        {
            movementSpeed = 6f;
        }
        else
        {
            movementSpeed = 5.5f;
        }

        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;

        speedBoosted = false;
        speedBoostTimer = 0f;
        disguiseActive = false;
        disguiseAlmostActive = false;
        disguiseTimer = 0f;

        if (isHunter)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (grounded)
        {
            RayDetection();

            if (runningAway && runAwayTimer < 1f)
            {
                runAwayTimer += Time.deltaTime;
            }
            else if (runningAway)
            {
                runningAway = false;
            }

            Collecting();

            SpeedBoost();

            Disguise();

            HoleInFloor();
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
            if (isHunter && (other.transform.gameObject.GetComponent<MeshRenderer>().material.color == Color.red || other.transform.gameObject.GetComponent<MeshRenderer>().material.color == Color.green))
            {

            }
            else
            {
                targets.Add(other.transform.parent.gameObject);
            }

        }
        else if (other.CompareTag("Runner") && other.GetComponentInChildren<MeshRenderer>().material.color == Color.yellow)
        {
            runners.Add(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Hunter") && other.GetComponentInChildren<MeshRenderer>().material.color == Color.red)
        {
            hunters.Add(other.transform.parent.gameObject);
            runningAway = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Runner") && other.GetComponentInChildren<MeshRenderer>().material.color == Color.yellow)
        {
            runners.Remove(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Hunter") && other.GetComponentInChildren<MeshRenderer>().material.color == Color.red)
        {
            hunters.Remove(other.transform.parent.gameObject);
        }
    }
    private void RayDetection()
    {
        // Rotate the mover if an object is detected in front
        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
        {
            transform.LookAt(transform.position - hitFront.normal);

            RotateAway();
        }
        else if (!isHunter && hunters.Count > 0 && looksLikeRunner && !runningAway)
        {
            List<Vector2> hunterDirections = new List<Vector2>();
            int numberOfSeenHunters = 0;
            foreach (GameObject hunter in hunters)
            {
                Physics.Linecast(transform.position + new Vector3(0, 0.1f, 0), hunter.transform.position + new Vector3(0, 0.1f, 0), out RaycastHit hit);

                //draw line straight to hunter to see if there is a wall in between

                if (hit.collider == null || !hit.collider.gameObject.CompareTag("Wall"))
                {
                    //no wall in between, do boxcast to see if the hunter can even reach the runner through the potential wall gap
                    Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), hunters[0].transform.position - transform.position, out hit, Quaternion.identity, Vector3.Distance(transform.position, hunters[0].transform.position));

                    //the hunter can reach the runner, so run away
                    if (hit.collider == null || hit.collider.gameObject.GetComponent<MeshRenderer>().material.color == Color.red)
                    {
                        hunterDirections.Add(new Vector2(hunter.transform.position.x - transform.position.x, hunter.transform.position.z - transform.position.z).normalized);
                        numberOfSeenHunters++;
                    }
                }
            }

            if (numberOfSeenHunters == 1)
            {
                transform.LookAt(hunters[0].transform.position);
                transform.Rotate(Vector3.up, 180);

                runningAway = true;
                runAwayTimer = 0f;
            }
            else if (numberOfSeenHunters > 1)
            {
                Vector2 avgDirection = Vector2.zero;
                foreach (Vector2 dir in hunterDirections)
                {
                    avgDirection += dir;
                }
                avgDirection /= numberOfSeenHunters;
                Vector3 lookTarget = new Vector3(transform.position.x - avgDirection.x, transform.position.y, transform.position.z - avgDirection.y);
                transform.LookAt(lookTarget);

                bool foundWayOut = false;
                int attempts = 0;
                while (!foundWayOut && !deactivateWayOutSearch)
                {
                    RaycastHit hit = new RaycastHit();
                    //need to cast a ray forward to see if there is a wall in the way
                    Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hit, Quaternion.identity, Vector3.Distance(transform.position, transform.forward * 5));

                    if (hit.collider != null && (hit.collider.gameObject.CompareTag("Wall") || hit.collider.gameObject.GetComponent<MeshRenderer>().material.color == Color.red))
                    {
                        transform.Rotate(Vector3.up, -10 * attempts);
                        attempts++;
                        if (attempts > 36)
                        {
                            deactivateWayOutSearch = true;
                            foundWayOut = true;
                        }
                    }
                    else
                    {
                        foundWayOut = true;
                    }
                }

                runningAway = true;
                runAwayTimer = 0f;
            }
        }
        else if (isHunter && runners.Count > 0)
        {
            Physics.Linecast(transform.position + new Vector3(0, 0.1f, 0), runners[0].transform.position + new Vector3(0, 0.1f, 0), out RaycastHit hit);


            if (hit.collider == null || !hit.collider.gameObject.CompareTag("Wall"))
            {
                Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), runners[0].transform.position - transform.position, out hit, Quaternion.identity, Vector3.Distance(transform.position, runners[0].transform.position));
                if (hit.collider == null || hit.collider.gameObject.CompareTag("Runner"))
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
                    if (!runningAway)
                    {
                        transform.LookAt(targets[0].transform.position);
                    }
                    
                }

        }
    }
    private void Collecting()
    {
        if (targets.Count > 0)
        {
            if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.5f)
            {
                deactivateWayOutSearch = false;

                //speed
                if (targets[0].GetComponentInChildren<MeshRenderer>().material.color == Color.yellow)
                {
                    movementSpeed *= 1.5f;
                    speedBoosted = true;
                    speedBoostTimer = 0f;
                    targets[0].SetActive(false);
                }
                //disguise
                else if (targets[0].GetComponentInChildren<MeshRenderer>().material.color == Color.green)
                {
                    if (!isHunter)
                    {
                        targets[0].SetActive(false);
                    }   
                }
                else if (targets[0].GetComponentInChildren<MeshRenderer>().material.color == Color.red)
                {
                    if (!isHunter)
                    {
                        disguiseAlmostActive = true;
                        disguiseTimer = 0f;
                        targets[0].SetActive(false);
                    }
                }
            }

            if (targets[0].activeSelf == false)
            {
                targets.RemoveAt(0);
            }
        }

        if(isHunter && runners.Count > 0)
        {
            if (Vector3.Distance(transform.position, runners[0].transform.position) < 3f)
            {
                runners[0].SetActive(false);
                
            }
        }

        if (isHunter && runners.Count > 0 && runners[0].activeSelf == false)
        {
            Destroy(runners[0]);
            runners.RemoveAt(0);
        }
    }
    private void HoleInFloor()
    {
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

    private void SpeedBoost()
    {
        // Handle speed boost duration
        if (speedBoosted && speedBoostTimer < 10)
        {
            speedBoostTimer += Time.deltaTime;
        }
        else if (speedBoosted)
        {
            speedBoosted = false;
            if (isHunter)
            {
                movementSpeed = 6f;
            }
            else
            {
                movementSpeed = 5.5f;
            }
        }
    }

    private void Disguise()
    {
        if (disguiseAlmostActive && runners.Count == 0 && hunters.Count == 0)
        {
            looksLikeRunner = false;


            GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            disguiseAlmostActive = false;
            disguiseActive = true;
        }
        else if (disguiseActive && disguiseTimer < 10)
        {
            disguiseTimer += Time.deltaTime;
        }
        //retain disguise if there are still targets nearby
        else if (disguiseActive && runners.Count == 0 && hunters.Count == 0)
        {
            disguiseActive = false;
            looksLikeRunner = true;


            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

        }


    }
}