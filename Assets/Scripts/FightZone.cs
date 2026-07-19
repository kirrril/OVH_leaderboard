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
}
