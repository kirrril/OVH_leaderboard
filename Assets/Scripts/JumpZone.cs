using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public Transform jumpingPos;
    public Transform chargingLowPos;
    public Transform chargingHighPos;
    [SerializeField] GameObject trampolinePhysicCollider;
    [SerializeField] private PlayerController playerController;
    public Animator selfAnimator;
    public string selfAnimatorBool;

    public enum JumpType { Plain, Charged }
    public JumpType jumpType;

    private void Update()
    {
        if (jumpType != JumpType.Charged) return;

        bool isChargingBounce =
        playerController.CurrentState == PlayerController.State.Jumping &&
        playerController.CurrentJumpPhase == PlayerController.JumpPhase.Charging;

        trampolinePhysicCollider.SetActive(!isChargingBounce);

        selfAnimator.SetBool(selfAnimatorBool, isChargingBounce);

        selfAnimator.SetFloat("bounceBlend", isChargingBounce ? playerController.JumpChargeBounce : 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.EnterJumpZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.ExitJumpZone(this);
    }
}
