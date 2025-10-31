using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] GameObject pickup;

    int pickupCount = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < pickupCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-45f, 46f), 0.5f, Random.Range(-46f, 46f));
            Instantiate(pickup, position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
