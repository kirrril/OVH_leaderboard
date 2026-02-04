using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform player;
    public PlayerController playerController;
    public Animator animator;
    public Transform[] trainingSpots;
    public GameObject fightZone;

    Transform spot;
    Transform trainingPos;
    Transform exitPos;
    GameObject wall;
    private string animBool = "";
    public float duration;
    private string scriptName;

    private int lastSpotIndex = -1;
    public bool hasInteracted;
    public bool isAvailable;

    void Awake()
    {
        player = GameObject.Find("PlayerPrefab").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateWalkingAnimation();
    }

    private void UpdateWalkingAnimation()
    {
        float speed = agent.velocity.magnitude;
        // animator.SetFloat("MovementSpeed", speed > 0.1f ? 1.9f : 0f);

    }

    public void MoveToTarget()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetNewTrainingSpot();
        }
    }

    public void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void SetNewTrainingSpot()
    {
        int newSpotIndex;
        do
        {
            newSpotIndex = Random.Range(0, trainingSpots.Length);
        } while (newSpotIndex == lastSpotIndex);

        lastSpotIndex = newSpotIndex;
        agent.SetDestination(trainingSpots[newSpotIndex].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        scriptName = other.tag;
        agent.ResetPath();

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
        if (!(bool)isAvailableField.GetValue(spotController))
        {
            isAvailable = true;
        }
        else
        {
            isAvailableField.SetValue(spotController, false);
            isAvailable = false;
        }
    }

    public void StartTraining()
    {
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;

        if (wall) wall.SetActive(true);
        animator.SetBool(animBool, true);

        hasInteracted = false;
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
