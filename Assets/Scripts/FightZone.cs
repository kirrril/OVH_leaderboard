using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.Framework;
using System;

public class FightZone : MonoBehaviour
{
    [SerializeField] private ManControllerFSM manController;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private Blackboard blackboard;
    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        if (GameManager.Instance.CurrentScore < 1)
        {
            return;
        }

        // if (GameManager.Instance.CurrentScore < 10)
        // {
        //     GameManager.Instance.ModifyScore(-1);
        //     manController.DoInsult();
        //     blackboard.SetVariableValue("hasInteracted", true);
        //     return;
        // }

        // playerController.ChangeState(PlayerController.State.Fighting);
        // manController.DoAttack();
        // navMeshObstacle.enabled = true;
        // blackboard.SetVariableValue("hasInteracted", true);
    }
}
