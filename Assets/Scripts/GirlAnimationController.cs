using UnityEngine;

public class GirlAnimationController : MonoBehaviour
{
    [SerializeField] private GirlController girlController;
    [SerializeField] private Animator animator;

    private void Update()
    {
        SyncTrainingType();
        SyncStateBools();
        SyncMoveBlend();
    }

    private void SyncTrainingType()
    {
        animator.SetInteger("trainingType", (int)girlController.CurrentTrainingType);
    }

    private void SyncStateBools()
    {
        GirlController.State state = girlController.CurrentState;

        animator.SetBool("isTraining", state == GirlController.State.Training);
        // animator.SetBool("isFleeing", state == GirlController.State.Fleeing);
        animator.SetBool("isWelcoming", state == GirlController.State.Welcoming);
    }

    private void SyncMoveBlend()
    {
        float target = 0f;

        switch (girlController.CurrentWalkingPhase)
        {
            case GirlController.WalkingPhase.Walking:
                target = 1f;
                break;

            case GirlController.WalkingPhase.Idle:
            case GirlController.WalkingPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("walkingSpeed", target, 0.1f, Time.deltaTime);
    }
}