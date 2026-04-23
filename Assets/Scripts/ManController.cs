using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour
{
    public Blackboard blackboard;
    public NavMeshAgent agent;
    private GameObject player;
    private GameObject agents;
    public PlayerController playerController;
    public Animator animator;
    private int level = 1;
    private GameObject patrolPoints;
    Transform[] trainingSpotsTransforms;
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
        player = GameObject.Find("Player");
        playerController = player.transform.Find("PlayerPrefab").GetComponent<PlayerController>();

        agents = GameObject.Find("Agents");

        switch (level)
        {
            case 1:
                patrolPoints = agents.transform.Find("Spots_man_legs").gameObject;
                break;
            case 2:
                patrolPoints = agents.transform.Find("Spots_man_chest").gameObject;
                break;
            case 3:
                patrolPoints = agents.transform.Find("Spots_man_back").gameObject;
                break;
        }

        trainingSpotsTransforms = patrolPoints.GetComponentsInChildren<Transform>();

        blackboard.SetVariableValue("player", playerController.gameObject);
    }

    void Update()
    {
        UpdateWalkingAnimation();
        blackboard.SetVariableValue("playerScore", playerController.score);
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
            newSpotIndex = Random.Range(0, trainingSpotsTransforms.Length);
        } while (newSpotIndex == lastSpotIndex);

        lastSpotIndex = newSpotIndex;

        Debug.Log(newSpotIndex);

        return newSpotIndex;
    }

    public void MoveToSpot()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(trainingSpotsTransforms[ChoseNewTrainingSpot()].position);
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

        if (!(bool)isAvailableField.GetValue(spotController)) return;

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
        playerController.score -= 1;
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
