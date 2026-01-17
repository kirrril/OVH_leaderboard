using UnityEngine;

public class Treadmill : MonoBehaviour
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
