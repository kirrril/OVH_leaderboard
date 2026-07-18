using UnityEngine;

public class ManAnimationController : MonoBehaviour
{
    [SerializeField] private ManController manController;
    [SerializeField] private Animator animator;

    private void Update()
    {
        SyncTrainingType();
        SyncStateBools();
        SyncMoveBlend();
    }

    private void SyncTrainingType()
    {
        animator.SetInteger("trainingType", (int)manController.CurrentTrainingType);
    }

    private void SyncStateBools()
    {
        ManController.State state = manController.CurrentState;

        animator.SetBool("isTraining", state == ManController.State.Training);
        // animator.SetBool("isChasing", state == ManController.State.Chasing);
        // animator.SetBool("isFleeing", state == ManController.State.Fleeing);
        animator.SetBool("isFighting", state == ManController.State.Fighting);
    }

    private void SyncMoveBlend()
    {
        float target = 0f;

        switch (manController.CurrentWalkingPhase)
        {
            case ManController.WalkingPhase.Walking:
                target = 1f;
                break;

            case ManController.WalkingPhase.Idle:
            case ManController.WalkingPhase.None:
                target = 0f;
                break;
        }

        animator.SetFloat("walkingSpeed", target, 0.1f, Time.deltaTime);
    }
}