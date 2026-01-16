using UnityEngine;

public class Bike : MonoBehaviour
{
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        // if (!other.CompareTag("Player")) return;
        isAvailable = false;
    }

    void OnTriggerExit(Collider other)
    {
        // if (!other.CompareTag("Player")) return;
        isAvailable = true;
    }
}
