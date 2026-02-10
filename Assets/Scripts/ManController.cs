using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour
{
    public Blackboard blackboard;
    public NavMeshAgent agent;
    private Transform player;
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
        player = GameObject.Find("PlayerPrefab").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();

        switch (level)
        {
            case 1:
                patrolPoints = GameObject.Find("Spots_man1");
                break;
            case 2:
                patrolPoints = GameObject.Find("Spots_man2");
                break;
            case 3:
                patrolPoints = GameObject.Find("Spots_man3");
                break;
        }

        trainingSpotsTransforms = patrolPoints.GetComponentsInChildren<Transform>();

        blackboard.SetVariableValue("player", player.gameObject);
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
        scriptName = other.tag;
        spot = other.transform;
        trainingPos = spot.Find("TrainingPos");
        exitPos = spot.Find("ExitPos");
        wall = spot.Find("Wall")?.gameObject;

        switch (scriptName)
        {
            case "Treadmill": animBool = "isJogging"; duration = 8f; break;
            case "Bike": animBool = "isCycling"; duration = 10f; break;
            case "JumpBox": animBool = "isBoxJumping"; duration = 7f; break;
            default: return;
        }

        var spotController = spot.GetComponent(scriptName);
        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if ((bool)isAvailableField.GetValue(spotController))
        {
            agent.ResetPath();
            blackboard.SetVariableValue("isTraining", true);
            blackboard.SetVariableValue("trainingDuration", duration);
            isAvailableField.SetValue(spotController, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        blackboard.SetVariableValue("isTraining", false);
        var spotController = spot.GetComponent(scriptName);
        var isAvailableField = spotController.GetType().GetField("isAvailable");
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
        transform.LookAt(player);
        fightZone.SetActive(true);
    }
}
