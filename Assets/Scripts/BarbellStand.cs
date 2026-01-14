using UnityEngine;

public class BarbellStand : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Man")) return;
        isAvailable = false;
        animator.SetBool("barbellStandIsMoving", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Man")) return;
        isAvailable = true;
        animator.SetBool("barbellStandIsMoving", false);
    }
}
