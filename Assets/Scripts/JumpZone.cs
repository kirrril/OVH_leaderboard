using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public Transform jumpingPos;
    private PlayerController playerController;
    public Animator selfAnimator;
    public string selfAnimatorBool;

    public enum JumpType { Plain, Charged }
    public JumpType jumpType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        playerController = other.gameObject.GetComponent<PlayerController>();
        playerController.EnterJumpZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        playerController = other.gameObject.GetComponent<PlayerController>();
        playerController.ExitJumpZone(this);
    }
}
