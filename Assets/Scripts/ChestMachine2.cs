using UnityEngine;

public class ChestMachine2 : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Girl")) return;
        isAvailable = false;
        animator.SetBool("chestMachine2IsMoving", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Girl")) return;
        isAvailable = true;
        animator.SetBool("chestMachine2IsMoving", false);
    }
}
