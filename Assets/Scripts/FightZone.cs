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
    private bool fightResolved;

    void Update()
    {
        navMeshObstacle.enabled = manController.CurrentState == ManController.State.Fighting;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.CurrentScore < 10) return;

        if (other != playerController.mainCollider) return;

        if (fightHasStarted) return;
        if (playerController.CurrentFightZone != null && playerController.CurrentFightZone != this) return;

        playerController.EnterFightZone(this);
        manController.ChangeState(ManController.State.Fighting);
        fightHasStarted = true;
        fightResolved = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != playerController.mainCollider) return;
        if (!fightHasStarted) return;
        if (!fightResolved) return;

        EndResolvedFight();
    }

    public void EndResolvedFight()
    {
        if (!fightHasStarted) return;
        if (!fightResolved) return;

        playerController.ExitFightZone();

        if (manController.CurrentState == ManController.State.Fighting)
        {
            manController.ChangeState(ManController.State.Patrol);
        }

        fightHasStarted = false;
        fightResolved = false;
    }

    public void HurtboxTouched(FightHitbox.AttackerType attackerType, FightHurtbox.HurtboxType hurtboxType)
    {
        if (!fightHasStarted) return;

        if (fightResolved) return;

        if (attackerType == FightHitbox.AttackerType.Player
            && playerController.CurrentFightPhase == PlayerController.FightPhase.BlockLeft
            || attackerType == FightHitbox.AttackerType.Player
            && playerController.CurrentFightPhase == PlayerController.FightPhase.BlockRight
            || attackerType == FightHitbox.AttackerType.Player
            && playerController.CurrentFightPhase == PlayerController.FightPhase.DuckDown)
        {
            return;
        }

        switch (attackerType)
        {
            case FightHitbox.AttackerType.Player:
                switch (hurtboxType)
                {
                    case FightHurtbox.HurtboxType.Chest:
                    case FightHurtbox.HurtboxType.HeadFront:
                        manController.ChangeFallDirection(ManController.FallDirection.Back);
                        manController.ChangeFightPhase(ManController.FightPhase.Defeat);
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        manController.ChangeFallDirection(ManController.FallDirection.Front);
                        manController.ChangeFightPhase(ManController.FightPhase.Defeat);
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        manController.ChangeFallDirection(ManController.FallDirection.Right);
                        manController.ChangeFightPhase(ManController.FightPhase.Defeat);
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        manController.ChangeFallDirection(ManController.FallDirection.Left);
                        manController.ChangeFightPhase(ManController.FightPhase.Defeat);
                        break;
                }
                playerController.ChangeFightPhase(PlayerController.FightPhase.Victory);
                fightResolved = true;
                break;
            case FightHitbox.AttackerType.Man:
                switch (hurtboxType)
                {
                    case FightHurtbox.HurtboxType.ArmLeft:
                        if (playerController.CurrentFightPhase != PlayerController.FightPhase.BlockLeft) break;
                        manController.ChangeFightPhase(ManController.FightPhase.None);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.None);
                        break;
                    case FightHurtbox.HurtboxType.ArmRight:
                        if (playerController.CurrentFightPhase != PlayerController.FightPhase.BlockRight) break;
                        manController.ChangeFightPhase(ManController.FightPhase.None);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.None);
                        break;
                    case FightHurtbox.HurtboxType.HeadFront:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Back);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        fightResolved = true;
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Front);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        fightResolved = true;
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Left);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        fightResolved = true;
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Right);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        fightResolved = true;
                        break;
                }
                break;
        }
    }
}
