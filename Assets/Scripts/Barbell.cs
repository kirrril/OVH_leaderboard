using UnityEngine;

public class Barbell : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isAvailable = false;
        animator.SetBool("barbellisMoving", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isAvailable = true;
        animator.SetBool("barbellisMoving", false);
    }
}