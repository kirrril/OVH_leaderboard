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
        SyncFightAction();
    }

    private void SyncStateBools()
    {
        ManController.State state = manController.CurrentState;

        animator.SetBool("isTraining", state == ManController.State.Training);
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

    private void SyncTrainingType()
    {
        animator.SetInteger("trainingType", (int)manController.CurrentTrainingType);
    }

    private void SyncFightAction()
    {
        int value = (int)manController.CurrentFightAction;
        animator.SetInteger("fightAction", value);
    }
}
