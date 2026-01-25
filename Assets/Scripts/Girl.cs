using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Girl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] trainingSpots;

    private int lastSpotIndex = -1;
    private float chaseFleeDistance = 3f;
    private float interactionDistance = 2.5f;
    private float chaseStopDistance = 6f;
    private float fleeStopDistance = 8f;
    private bool hasInteracted;

    private enum State { MovingToTarget, Training, FleeingChasing, Interacting }
    private State currentState = State.MovingToTarget;

    void Update()
    {
        UpdateWalkingAnimation();

        switch (currentState)
        {
            case State.MovingToTarget:
                HandleMovingToTarget();
                break;

            case State.FleeingChasing:
                HandleFleeingChasing();
                break;

            case State.Interacting:
                HandleInteracting();
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
        hasInteracted = false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseFleeDistance)
        {
            currentState = State.FleeingChasing;
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetNewTarget();
        }
    }

    private void HandleFleeingChasing()
    {
        if (playerController.score < 50)
        {
            Flee();
        }
        else
        {
            Chase();
        }
    }

    private void Flee()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < interactionDistance) currentState = State.Interacting;
        if (distanceToPlayer > fleeStopDistance)
        {
            currentState = State.MovingToTarget;
            return;
        }

        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 target = transform.position + dirAway * 6f;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void Chase()
    {
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < interactionDistance) currentState = State.Interacting;
        if (distanceToPlayer > chaseStopDistance)
        {
            currentState = State.MovingToTarget;
        }
    }

    private void HandleInteracting()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > interactionDistance) currentState = State.FleeingChasing;

        if (hasInteracted) return;

        if (playerController.score < 50)
        {
            StartCoroutine(DoInteract("Fuck off loser!", -1));
        }
        else
        {
            StartCoroutine(DoInteract("Hello darling!", +1));
        }
    }

    private void SetNewTarget()
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
        if (currentState == State.Training) return;

        string tag = other.tag;

        Transform spot = other.transform;
        Transform trainingPos = spot.Find("TrainingPos");
        Transform exitPos = spot.Find("ExitPos");
        GameObject wall = spot.Find("Wall")?.gameObject;

        string animBool = "";
        int duration = 0;
        string scriptName = tag;

        switch (tag)
        {
            case "Treadmill": animBool = "isJogging"; duration = 8; break;
            case "Bike": animBool = "isCycling"; duration = 10; break;
            case "JumpBox": animBool = "isBoxJumping"; duration = 7; break;
            default: return;
        }

        var spotController = spot.GetComponent(scriptName);

        var isAvailableField = spotController.GetType().GetField("isAvailable");
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

        StartCoroutine(DoTraining(wall, trainingPos, exitPos, animBool, duration));
    }

    private IEnumerator DoTraining(GameObject wall, Transform trainingPos, Transform exitPos, string animBool, int duration)
    {
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;

        if (wall) wall.SetActive(true);
        animator.SetBool(animBool, true);

        yield return new WaitForSeconds(duration);

        if (wall) wall.SetActive(false);
        animator.SetBool(animBool, false);

        transform.position = exitPos.position;
        transform.rotation = exitPos.rotation;

        agent.enabled = true;
        agent.isStopped = false;

        currentState = State.MovingToTarget;
    }

    private IEnumerator DoInteract(string message, int scoreDelta)
    {
        Debug.Log(message);
        playerController.score += scoreDelta;
        hasInteracted = true;
        yield return new WaitForSeconds(1f);
        currentState = State.MovingToTarget;
    }
}