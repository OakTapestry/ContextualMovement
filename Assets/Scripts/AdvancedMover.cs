using UnityEngine;

public class AdvancedMover : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    RaycastHit hitFront, hitLeft, hitRight;
    [SerializeField] float forwardDist, sideDist, downDist;
    bool leftWall, rightWall;

    int randInt;

    [SerializeField] GameObject downCheck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = 3.0f;
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

    }

    // Update is called once per frame
    private void Update()
    {
        // Rotate the mover if an object is detected in front
        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
        {
            transform.LookAt(transform.position - hitFront.normal);

            RotateAway();
        }

        // Rotate the mover if a hole is detected in front
        if (!Physics.BoxCast(downCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
        {       
            RotateAway();
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
            Debug.Log("Right");
        }
        else if (!leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, -90);
            Debug.Log("Left");
        }
        else if (leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, 180);
            Debug.Log("Back");
        }
        else
        {
            randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                transform.Rotate(Vector3.up, 90);
                Debug.Log("RightRand");
            }
            else
            {
                transform.Rotate(Vector3.up, -90);
                Debug.Log("LeftRand");
            }

        }
    }
}
