using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private int randomValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomValue = Random.Range(0, 100);

        if(randomValue > 0 && randomValue <= 10)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
        else if(randomValue > 10 && randomValue <= 20 )
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            transform.position += new Vector3(10f, 0, 10f);
            if(transform.position.x > 46)
            {
                transform.position += new Vector3(-20f, 0, 0);
            }
            if(transform.position.z > 46)
            {
                transform.position += new Vector3(0, 0, -20f);
            }
        }
    }
}
