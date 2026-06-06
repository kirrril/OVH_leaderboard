using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour, IAgent
{
    public Blackboard blackboard;
    public NavMeshAgent agent;
    [SerializeField] private GameObject agents;
    public PlayerController playerController;
    public Animator animator;
    private Transform targetSpot;
    [SerializeField] Transform[] trainingSpots;
    int lastSpotIndex = -1;
    public GameObject fightZone;


    void Awake()
    {
        blackboard.SetVariableValue("player", playerController.gameObject);

        agent.avoidancePriority = Random.Range(45, 99);
    }

    void Update()
    {
        UpdateWalkingAnimation();
        blackboard.SetVariableValue("playerScore", GameManager.Instance.CurrentScore);
    }

    private void UpdateWalkingAnimation()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("SpeedMagnitude", speed > 0.1f ? 1.9f : 0f);

    }

    private int ChoseNewTrainingSpot()
    {
        int newSpotIndex;
        do
        {
            newSpotIndex = Random.Range(0, trainingSpots.Length);
        } while (newSpotIndex == lastSpotIndex);

        lastSpotIndex = newSpotIndex;

        return newSpotIndex;
    }

    public void MoveToSpot()
    {
        if (!agent.hasPath)
        {
            int spotIndex = ChoseNewTrainingSpot();
            if (spotIndex < 0) return;

            targetSpot = trainingSpots[spotIndex];

            agent.SetDestination(targetSpot.position);
        }
    }

    public void CancelTraining()
    {
        agent.ResetPath();
    }

    public void StartTraining(TrainingData trainingData)
    {
        agent.isStopped = true;
        agent.enabled = false;
        blackboard.SetVariableValue("isTraining", true);
        blackboard.SetVariableValue("hasInteracted", false);
        transform.position = trainingData.trainingPos.position;
        transform.rotation = trainingData.trainingPos.rotation;
        animator.SetBool(trainingData.userAnimatorBool, true);
    }

    public void StopTraining(TrainingData trainingData)
    {
        animator.SetBool(trainingData.userAnimatorBool, false);
        transform.position = trainingData.exitPos.position;
        transform.rotation = trainingData.exitPos.rotation;
        agent.enabled = true;
        agent.isStopped = false;
        blackboard.SetVariableValue("isTraining", false);
    }

    public void DoInsult()
    {
        Debug.Log("You little nerd!");
        GameManager.Instance.ModifyScore(-1);
    }

    public void DoAttack()
    {
        playerController.isBeingAttacked = true;
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        transform.LookAt(playerController.transform);
        fightZone.SetActive(true);
    }
}
