using UnityEngine;

public class Rower : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Man")) return;
        isAvailable = false;
        animator.SetBool("RowerIsMoving", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Man")) return;
        isAvailable = true;
        animator.SetBool("RowerIsMoving", false);
    }
}
