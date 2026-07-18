using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FightZone : MonoBehaviour
{
    [SerializeField] private ManController manController;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.CurrentScore < 10)
        {
            return;
        }
    }
}
