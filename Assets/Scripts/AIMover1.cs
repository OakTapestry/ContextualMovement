using UnityEngine;

public class AIMover1 : MonoBehaviour
{
    [SerializeField] float movementSpeed, forwardDist, sideDist;

    bool detectLeft, detectRight;

    RaycastHit hit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = 3.0f;
        forwardDist = 0.5f;
        sideDist = 3.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        //check for objects in front
        if (Physics.BoxCast(transform.position + new Vector3(0,1,0), new Vector3(0.5f, 1, 0.5f), transform.forward, out hit, Quaternion.identity, forwardDist))
        {
            detectLeft = false;
            detectRight = false;
            //check for objects to the left and right
            if (Physics.BoxCast(transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0.5f), -transform.right, Quaternion.identity, sideDist))
            {
                detectLeft = true;
            }
            if (Physics.BoxCast(transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0.5f), transform.right, Quaternion.identity, sideDist))
            {
                detectRight = true;
            }

            if (detectLeft && !detectRight)
            {
                transform.Rotate(Vector3.up, 90);
            }
            else if (!detectLeft && detectRight)
            {
                transform.Rotate(Vector3.up, -90);
            }
            else if (detectLeft && detectRight)
            {
                transform.Rotate(Vector3.up, 180);
            }
            else
            {
                int randDir;
                randDir = Random.Range(0, 2);
                if (randDir == 0)
                {
                    transform.Rotate(Vector3.up, 90);
                }
                else
                {
                    transform.Rotate(Vector3.up, -90);
                }
            }
            
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.fixedDeltaTime);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.tag == "Wall")
    //    {
    //        transform.Rotate(Vector3.up, 90);
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Wall")
    //    {
    //        transform.Rotate(Vector3.up, 90);
    //    }        
    //}
}
