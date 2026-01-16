using UnityEngine;

public class Dips : MonoBehaviour
{
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Girl")) return;
        isAvailable = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Girl")) return;
        isAvailable = true;
    }
}
