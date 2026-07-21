using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FightZone : MonoBehaviour
{
    [SerializeField] private ManController manController;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    private bool fightHasStarted;

    void Update()
    {
        navMeshObstacle.enabled = manController.CurrentState == ManController.State.Fighting;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.CurrentScore < 10) return;

        if (other.tag != "Player") return;

        playerController.EnterFightZone(this);
        manController.ChangeState(ManController.State.Fighting);

        fightHasStarted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;
        if (!fightHasStarted) return;

        playerController.ExitFightZone();
        manController.ChangeState(ManController.State.Patrol);

        fightHasStarted = false;
    }

    public void HurtboxTouched(FightHitbox.AttackerType attackerType, FightHurtbox.HurtboxType hurtboxType)
    {
        switch (attackerType)
        {
            case FightHitbox.AttackerType.Player:
                manController.ChangeFightPhase(ManController.FightPhase.Defeat);
                switch (hurtboxType)
                {
                    case FightHurtbox.HurtboxType.Chest:
                    case FightHurtbox.HurtboxType.HeadFront:
                        manController.ChangeFallDirection(ManController.FallDirection.Back);
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        manController.ChangeFallDirection(ManController.FallDirection.Front);
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        manController.ChangeFallDirection(ManController.FallDirection.Right);
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        manController.ChangeFallDirection(ManController.FallDirection.Left);
                        break;
                }
                playerController.ChangeFightPhase(PlayerController.FightPhase.Victory);
                break;
            case FightHitbox.AttackerType.Man:
                switch (hurtboxType)
                {
                    case FightHurtbox.HurtboxType.ArmLeft:
                    case FightHurtbox.HurtboxType.ArmRight:
                        break;
                    case FightHurtbox.HurtboxType.HeadFront:
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Back);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Front);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Right);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Left);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        break;
                }
                break;
        }
    }
}
