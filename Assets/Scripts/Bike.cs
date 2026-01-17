using UnityEngine;

public class Bike : MonoBehaviour
{
    public bool isAvailable;

    void Start()
    {
        isAvailable = true;
    }

    void OnTriggerExit(Collider other)
    {
        isAvailable = true;
    }
}
