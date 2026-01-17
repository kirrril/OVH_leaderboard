using UnityEngine;

public class JumpBox : MonoBehaviour
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
