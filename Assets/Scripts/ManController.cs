using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour
{
    public Blackboard blackboard;
    public NavMeshAgent agent;
    [SerializeField] private GameObject agents;
    public PlayerController playerController;
    public Animator animator;
    private Transform targetSpot;
    private int level = 1;
    [SerializeField] private Transform[] spotsManLegs;
    [SerializeField] private Transform[] spotsManChest;
    [SerializeField] private Transform[] spotsManBack;
    Transform[] trainingSpots;
    int lastSpotIndex = -1;
    public GameObject fightZone;

    Transform spot;
    Transform trainingPos;
    Transform exitPos;
    GameObject wall;
    private string animBool = "";
    public float duration;
    private string scriptName;

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
        blackboard.SetVariableValue("playerScore", PlayerData.Instance.currentScore);
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

    public bool CheckIfAvailable(Transform spot)
    {
        var spotController = spot.GetComponent(spot.tag);
        if (spotController == null) return false;

        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return false;

        return (bool)isAvailableField.GetValue(spotController);
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

        if (agent.remainingDistance < 0.2f)
        {
            if (!CheckIfAvailable(targetSpot))
            {
                agent.ResetPath();
                targetSpot = null;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string enteringTag = other.tag;

        switch (enteringTag)
        {
            case "Treadmill":
                animBool = "isJogging";
                duration = 8f;
                break;

            case "Bike":
                animBool = "isCycling";
                duration = 10f;
                break;

            case "JumpBox":
                animBool = "isBoxJumping";
                duration = 7f;
                break;

            default:
                return;
        }

        Transform enteringSpot = other.transform;
        Transform enteringTrainingPos = enteringSpot.Find("TrainingPos");
        Transform enteringExitPos = enteringSpot.Find("ExitPos");
        GameObject enteringWall = enteringSpot.Find("Wall")?.gameObject;

        var spotController = other.GetComponent(enteringTag);
        if (spotController == null) return;

        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return;

        if (!(bool)isAvailableField.GetValue(spotController))
        {
            agent.ResetPath();
            MoveToSpot();
            return;
        }

        spot = enteringSpot;
        trainingPos = enteringTrainingPos;
        exitPos = enteringExitPos;
        wall = enteringWall;
        scriptName = enteringTag;

        agent.ResetPath();
        blackboard.SetVariableValue("isTraining", true);
        blackboard.SetVariableValue("trainingDuration", duration);
        isAvailableField.SetValue(spotController, false);
    }

    private void OnTriggerExit(Collider other)
    {
        string exitingTag = other.tag;

        switch (exitingTag)
        {
            case "Treadmill":
            case "Bike":
            case "JumpBox":
                break;
            default:
                return;
        }

        var spotController = other.GetComponent(exitingTag);
        if (spotController == null) return;

        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return;

        isAvailableField.SetValue(spotController, true);
    }

    public void StartTraining()
    {
        agent.isStopped = true;
        agent.enabled = false;
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;
        if (wall) wall.SetActive(true);
        animator.SetBool(animBool, true);
    }

    public void StopTraining()
    {
        if (wall) wall.SetActive(false);
        animator.SetBool(animBool, false);
        transform.position = exitPos.position;
        transform.rotation = exitPos.rotation;
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
