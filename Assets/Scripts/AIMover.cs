using UnityEngine;

public class AIMover : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    RaycastHit hitFront, hitLeft, hitRight;
    [SerializeField] float forwardDist, sideDist;
    bool leftWall, rightWall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = 3.0f;
        forwardDist = 1.0f;
        sideDist = 2.0f;

    }

    // Update is called once per frame
    private void Update()
    {
        if (Physics.BoxCast(transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
        {
            if (Physics.BoxCast(transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0.5f), -transform.right, out hitLeft, Quaternion.identity, sideDist))
            {
                leftWall = true;
            }

            if (Physics.BoxCast(transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0.5f), transform.right, out hitRight, Quaternion.identity, sideDist))
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
                transform.Rotate(Vector3.up, 90);
            }
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.fixedDeltaTime);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Wall")
    //    {
    //        transform.Rotate(Vector3.up, 90);
    //    }
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.tag == "Wall")
    //    {
    //        transform.Rotate(Vector3.up, 90);
    //    }
    //}
}
