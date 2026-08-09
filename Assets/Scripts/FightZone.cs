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
        if (playerController.CurrentSecurityZone != null) return;

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

    private void ResolvePlayerWin()
    {
        playerController.ChangeFightPhase(PlayerController.FightPhase.Victory);
        manController.ChangeFightPhase(ManController.FightPhase.Defeat);
        GameManager.Instance.ModifyScore(10);
        fightResolved = true;
    }

    private void ResolveManWin()
    {
        manController.ChangeFightPhase(ManController.FightPhase.Victory);
        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
        GameManager.Instance.ModifyScore(-10);
        fightResolved = true;
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
                        ResolvePlayerWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        manController.ChangeFallDirection(ManController.FallDirection.Front);
                        ResolvePlayerWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        manController.ChangeFallDirection(ManController.FallDirection.Right);
                        ResolvePlayerWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        manController.ChangeFallDirection(ManController.FallDirection.Left);
                        ResolvePlayerWin();
                        break;
                }
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
                        ResolveManWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Front);
                        ResolveManWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Left);
                        ResolveManWin();
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Right);
                        ResolveManWin();
                        break;
                }
                break;
        }
    }
}
