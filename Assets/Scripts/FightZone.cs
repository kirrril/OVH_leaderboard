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

        if (fightHasStarted) return;
        if (playerController.CurrentFightZone != null && playerController.CurrentFightZone != this) return;

        playerController.EnterFightZone(this);
        manController.ChangeState(ManController.State.Fighting);
        fightHasStarted = true;
        manController.SetFightResolved(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;
        if (!fightHasStarted) return;
        // if (!manController.IsFightResolved) return;

        playerController.ExitFightZone();

        if (manController.CurrentState == ManController.State.Fighting)
        {
            manController.ChangeState(ManController.State.Patrol);
        }

        fightHasStarted = false;
        manController.SetFightResolved(true);
    }

    public void HurtboxTouched(FightHitbox.AttackerType attackerType, FightHurtbox.HurtboxType hurtboxType)
    {
        if (!fightHasStarted) return;

        if (manController.IsFightResolved) return;

        if (attackerType == FightHitbox.AttackerType.Player // ________________ éviter les coups dans le ventre
        && hurtboxType == FightHurtbox.HurtboxType.Chest
        && (playerController.CurrentFightPhase != PlayerController.FightPhase.Attack
        || playerController.CurrentFightSide != PlayerController.FightSide.Front))
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
                manController.SetFightResolved(true);
                break;
            case FightHitbox.AttackerType.Man:
                switch (hurtboxType)
                {
                    case FightHurtbox.HurtboxType.ArmLeft:
                        if (playerController.CurrentFightPhase != PlayerController.FightPhase.Block) break;
                        if (playerController.CurrentFightSide != PlayerController.FightSide.Left) break;
                        manController.ChangeFightPhase(ManController.FightPhase.None);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.None);
                        break;
                    case FightHurtbox.HurtboxType.ArmRight:
                        if (playerController.CurrentFightPhase != PlayerController.FightPhase.Block) break;
                        if (playerController.CurrentFightSide != PlayerController.FightSide.Right) break;
                        manController.ChangeFightPhase(ManController.FightPhase.None);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.None);
                        break;
                    case FightHurtbox.HurtboxType.HeadFront:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Back);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        manController.SetFightResolved(true);
                        break;
                    case FightHurtbox.HurtboxType.HeadBack:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Front);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        manController.SetFightResolved(true);
                        break;
                    case FightHurtbox.HurtboxType.HeadLeft:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Left);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        manController.SetFightResolved(true);
                        break;
                    case FightHurtbox.HurtboxType.HeadRight:
                        playerController.ChangeFallDirection(PlayerController.FallDirection.Right);
                        playerController.ChangeFightPhase(PlayerController.FightPhase.Defeat);
                        manController.ChangeFightPhase(ManController.FightPhase.Victory);
                        manController.SetFightResolved(true);
                        break;
                }
                break;
        }
        playerController.ChangeFightSide(PlayerController.FightSide.None);
    }
}
