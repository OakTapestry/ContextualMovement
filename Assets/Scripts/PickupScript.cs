using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private int randomValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomValue = Random.Range(0, 3);

        if(randomValue == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
        else if(randomValue == 1)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
        else if(randomValue == 2)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        
    }
}
