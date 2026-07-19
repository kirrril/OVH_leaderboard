using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    private PlayerController.JumpPhase lastJumpPhase = PlayerController.JumpPhase.None;

    private void Update()
    {
        SyncTrainingType();
        SyncStateBools();
        SyncMoveBlend();
        SyncJumpBlend();
        SyncLandingTrigger();
        SyncDoorBlend();
        SyncClimbBlend();
    }

    private void SyncTrainingType()
    {
        int value = 0;

        switch (playerController.CurrentTrainingType)
        {
            case PlayerTrainingType.PullUps:
                if (GameManager.Instance.PullUpsTraining < 0.33f)
                {
                    value = 13;
                    break;
                }
                if (GameManager.Instance.PullUpsTraining > 0.66f)
                {
                    value = 15;
                    break;
                }
                value = 14;
                break;
            default:
                value = (int)playerController.CurrentTrainingType;
                break;
        }

        animator.SetInteger("trainingType", value);
    }

    private void SyncStateBools()
    {
        PlayerController.State state = playerController.CurrentState;

        animator.SetBool("isGaming", state == PlayerController.State.Gaming);
        animator.SetBool("isTraining", state == PlayerController.State.Training);
        animator.SetBool("isJumping", state == PlayerController.State.Jumping);
        animator.SetBool("isFalling", state == PlayerController.State.Falling);
        animator.SetBool("isPushingTheDoor", state == PlayerController.State.PushingTheDoor);
        animator.SetBool("isClimbing", state == PlayerController.State.ClimbingThePole);
    }

    private void SyncMoveBlend()
    {
        float target = 0f;

        switch (playerController.CurrentWalkingPhase)
        {
            case PlayerController.WalkingPhase.Walking:
                target = 1f;
                break;

            case PlayerController.WalkingPhase.Idle:
            case PlayerController.WalkingPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("walkingSpeed", target, 0.1f, Time.deltaTime);
    }

    private void SyncJumpBlend()
    {
        float target = 0f;

        switch (playerController.CurrentJumpPhase)
        {
            case PlayerController.JumpPhase.Charging:
                target = playerController.JumpChargeBounce;
                break;

            case PlayerController.JumpPhase.Squatting:
                target = 0f;
                break;

            case PlayerController.JumpPhase.Released:
                target = 1f;
                break;

            case PlayerController.JumpPhase.Airborne:
                target = 2f;
                break;

            case PlayerController.JumpPhase.Landed:
                target = 2f;
                break;

            case PlayerController.JumpPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("jumpState", target);
    }

    private void SyncLandingTrigger()
    {
        PlayerController.JumpPhase currentJumpPhase = playerController.CurrentJumpPhase;

        if (currentJumpPhase == PlayerController.JumpPhase.Landed &&
            lastJumpPhase != PlayerController.JumpPhase.Landed)
        {
            animator.SetTrigger("land");
        }

        lastJumpPhase = currentJumpPhase;
    }

    private void SyncDoorBlend()
    {
        float target = 0f;

        switch (playerController.CurrentDoorPhase)
        {
            case PlayerController.DoorPhase.Pushing:
                target = 0f;
                break;

            case PlayerController.DoorPhase.Releasing:
                target = 1f;
                break;

            case PlayerController.DoorPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("pushingState", target);
    }

    private void SyncClimbBlend()
    {
        float target = 0f;

        switch (playerController.CurrentClimbPhase)
        {
            case PlayerController.ClimbPhase.SlidingDown:
                target = 0f;
                break;

            case PlayerController.ClimbPhase.ClimbingUp:
                target = 1f;
                break;

            case PlayerController.ClimbPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("climbingState", target);
    }
}