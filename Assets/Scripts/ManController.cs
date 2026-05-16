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
    private int level = 2;
    [SerializeField] private Transform[] spotsManLegs;
    [SerializeField] private Transform[] spotsManChest;
    [SerializeField] private Transform[] spotsManBack;
    Transform[] trainingSpots;
    int lastSpotIndex = -1;
    public GameObject fightZone;


    void Awake()
    {
        switch (level)
        {
            case 1:
                trainingSpots = spotsManLegs;
                break;
            case 2:
                trainingSpots = spotsManChest;
                break;
            case 3:
                trainingSpots = spotsManBack;
                break;
        }
        blackboard.SetVariableValue("player", playerController.gameObject);

        agent.avoidancePriority = Random.Range(45, 99);
    }

    void Update()
    {
        UpdateWalkingAnimation();
        blackboard.SetVariableValue("playerScore", GameManager.Instance.currentScore);
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

    public void ResetPath()
    {
        agent.ResetPath();
    }

    public void StartTraining(TrainingSpot trainingSpot)
    {
        agent.isStopped = true;
        agent.enabled = false;
        blackboard.SetVariableValue("isTraining", true);
        blackboard.SetVariableValue("hasInteracted", false);
        transform.position = trainingSpot.trainingPos.position;
        transform.rotation = trainingSpot.trainingPos.rotation;
        animator.SetBool(trainingSpot.userAnimatorBool, true);
    }

    public void StopTraining(TrainingSpot trainingSpot)
    {
        animator.SetBool(trainingSpot.userAnimatorBool, false);
        transform.position = trainingSpot.exitPos.position;
        transform.rotation = trainingSpot.exitPos.rotation;
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
