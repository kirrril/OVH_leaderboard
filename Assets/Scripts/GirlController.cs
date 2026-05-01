using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;

public class GirlController : MonoBehaviour
{
    private enum SceneMode { Gameplay, YouWin }
    private enum State { MovingToTarget, Training, Fleeing, Welcoming }

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] trainingSpots;
    private SceneMode sceneMode = SceneMode.Gameplay;

    private int lastSpotIndex = -1;
    private float awarenessDistance = 3f;
    private float interactionDistance = 2.5f;
    private float fleeStopDistance = 4f;
    private float welcomingStopDistance = 4f;
    private bool hasInteracted;
    private bool isPerformingInteraction;

    private State currentState = State.MovingToTarget;

    void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "YouWinScene":
                sceneMode = SceneMode.YouWin;
                break;
            default:
                sceneMode = SceneMode.Gameplay;
                break;
        }

        if (playerController == null)
        {
            GameObject playerRoot = GameObject.Find("Player");
            if (playerRoot != null)
            {
                playerController = playerRoot.GetComponentInChildren<PlayerController>();
            }
        }

        if (player == null && playerController != null)
        {
            player = playerController.transform;
        }

        agent.avoidancePriority = Random.Range(0, 44);
    }

    void Update()
    {
        UpdateWalkingAnimation();

        if (player == null || playerController == null || isPerformingInteraction) return;

        switch (currentState)
        {
            case State.MovingToTarget:
                HandleMovingToTarget();
                break;

            case State.Fleeing:
                HandleFleeing();
                break;

            case State.Welcoming:
                HandleWelcoming();
                break;

            case State.Training:
                break;
        }
    }

    private void UpdateWalkingAnimation()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("MovementSpeed", speed > 0.1f ? 1.9f : 0f);
    }

    private void HandleMovingToTarget()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < awarenessDistance && !hasInteracted)
        {
            currentState = sceneMode == SceneMode.YouWin ? State.Welcoming : State.Fleeing;
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetNewTarget();
        }
    }

    private void HandleFleeing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < interactionDistance)
        {
            StartCoroutine(DoInsult("Fuck off loser!", -1));
            return;
        }

        if (distanceToPlayer > fleeStopDistance)
        {
            currentState = State.MovingToTarget;
            agent.ResetPath();
            return;
        }

        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 target = transform.position + dirAway * 6f;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void HandleWelcoming()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < interactionDistance)
        {
            StartCoroutine(DoWelcome("Hello honey!"));
            return;
        }

        if (distanceToPlayer > welcomingStopDistance)
        {
            currentState = State.MovingToTarget;
            agent.ResetPath();
            return;
        }

        agent.SetDestination(player.position);
    }

    private void SetNewTarget()
    {
        if (trainingSpots == null || trainingSpots.Length == 0) return;

        if (trainingSpots.Length == 1)
        {
            lastSpotIndex = 0;
            agent.SetDestination(trainingSpots[0].position);
            return;
        }

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
        if (currentState == State.Training || isPerformingInteraction) return;

        string tag = other.tag;

        Transform spot = other.transform;
        Transform trainingPos = spot.Find("TrainingPos");
        Transform exitPos = spot.Find("ExitPos");
        GameObject wall = spot.Find("Wall")?.gameObject;

        if (trainingPos == null || exitPos == null) return;

        string animBool = "";
        int duration = 0;

        switch (tag)
        {
            case "Treadmill": animBool = "isJogging"; duration = 8; break;
            case "Bike": animBool = "isCycling"; duration = 10; break;
            case "JumpBox": animBool = "isBoxJumping"; duration = 7; break;
            default: return;
        }

        var spotController = other.GetComponent(tag);
        if (spotController == null) return;

        FieldInfo isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return;

        if (!(bool)isAvailableField.GetValue(spotController))
        {
            agent.ResetPath();
            SetNewTarget();
            return;
        }

        currentState = State.Training;
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;

        isAvailableField.SetValue(spotController, false);

        StartCoroutine(DoTraining(wall, trainingPos, exitPos, animBool, duration, spotController, isAvailableField));
    }

    private IEnumerator DoTraining(GameObject wall, Transform trainingPos, Transform exitPos, string animBool, int duration, object spotController, FieldInfo isAvailableField)
    {
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;

        if (wall) wall.SetActive(true);
        animator.SetBool(animBool, true);

        hasInteracted = false;

        yield return new WaitForSeconds(duration);

        if (wall) wall.SetActive(false);
        animator.SetBool(animBool, false);

        transform.position = exitPos.position;
        transform.rotation = exitPos.rotation;

        isAvailableField?.SetValue(spotController, true);

        agent.enabled = true;
        agent.isStopped = false;

        currentState = State.MovingToTarget;
    }

    private IEnumerator DoInsult(string message, int scoreDelta)
    {
        if (hasInteracted) yield break;

        hasInteracted = true;
        isPerformingInteraction = true;
        agent.ResetPath();
        agent.isStopped = true;
        Debug.Log(message);
        GameManager.Instance.ModifyScore(scoreDelta);
        yield return new WaitForSeconds(0.1f);
        agent.isStopped = false;
        isPerformingInteraction = false;
        currentState = State.MovingToTarget;
    }

    private IEnumerator DoWelcome(string message)
    {
        if (hasInteracted) yield break;

        hasInteracted = true;
        isPerformingInteraction = true;
        agent.ResetPath();
        agent.isStopped = true;
        Debug.Log(message);
        // Hook for a future DoubleSelfieZone / cutscene flow in YouWin.
        yield return new WaitForSeconds(0.1f);
        agent.isStopped = false;
        isPerformingInteraction = false;
        currentState = State.MovingToTarget;
    }
}
