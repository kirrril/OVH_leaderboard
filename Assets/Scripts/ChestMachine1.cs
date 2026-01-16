using UnityEngine;

public class ChestMachine1 : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        // if (!other.CompareTag("Player")) return;
        isAvailable = false;
        animator.SetBool("chestMachineIsMoving", true);
    }

    void OnTriggerExit(Collider other)
    {
        // if (!other.CompareTag("Player")) return;
        isAvailable = true;
        animator.SetBool("chestMachineIsMoving", false);
    }
}
