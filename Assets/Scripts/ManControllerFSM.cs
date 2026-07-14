using UnityEngine;
using UnityEngine.AI;

public class ManControllerFSM : MonoBehaviour, IAgent
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerController playerController;
    public Animator animator;
    private Transform targetSpot;
    [SerializeField] Transform[] trainingSpots;
    private int lastSpotIndex = -1;

    [SerializeField] Collider[] fightColliders;

    [SerializeField] private int agentPriority;

    private float awarenessDistance = 4f;
    private float awarenessAngle = 120f;
    private float fleeDistance = 3f;
    public bool wasBeaten;

    public enum State { Patrol, Chasing, Fleeing, Training, Fighting }
    public enum WalkingPhase { None, Idle, Walking };
    public enum FightPhase { None, Attack, Defeat, Victory }

    public State CurrentState { get; private set; } = State.Patrol;
    private TrainingData currentTrainingData;
    public WalkingPhase CurrentWalkingPhase { get; private set; }

    void Awake()
    {
        agent.avoidancePriority = agentPriority;
        SwitchFightColliders("Off");
    }

    void Update()
    {
        switch (CurrentState)
        {
            case State.Patrol:
                if (CanChase())
                {
                    ChangeState(State.Chasing);
                    return;
                }
                if (NeedToFlee())
                {
                    ChangeState(State.Fleeing);
                    return;
                }
                HandlePatrol();
                break;
            case State.Chasing:
                if (!CanChase())
                {
                    ChangeState(State.Patrol);
                    return;
                }
                HandleChasing();
                break;
            case State.Fleeing:
                if (!NeedToFlee())
                {
                    ChangeState(State.Patrol);
                    return;
                }
                HandleFleeing();
                break;
            case State.Training:
                HandleTraining();
                break;
            case State.Fighting:
                HandleFighting();
                break;
        }
    }

    private void ChangeState(State nextState)
    {
        if (CurrentState == nextState) return;

        ExitState(CurrentState);
        CurrentState = nextState;
        EnterState(CurrentState);
    }

    private void ExitState(State state)
    {
        switch (state)
        {
            case State.Fighting:
                SwitchFightColliders("Off");
                break;
        }
    }

    private void EnterState(State state)
    {
        switch (state)
        {
            case State.Patrol:
                agent.enabled = true;
                agent.isStopped = false;
                agent.ResetPath();
                MoveToSpot();
                break;

            case State.Chasing:
                agent.enabled = true;
                agent.isStopped = false;
                agent.ResetPath();
                break;

            case State.Training:
                wasBeaten = false;
                agent.isStopped = true;
                agent.ResetPath();
                agent.enabled = false;
                transform.position = currentTrainingData.trainingPos.position;
                transform.rotation = currentTrainingData.trainingPos.rotation;
                break;

            case State.Fighting:
                agent.ResetPath();
                agent.isStopped = true;
                agent.enabled = false;
                transform.LookAt(playerController.transform);
                SwitchFightColliders("On");
                break;

            case State.Fleeing:
                agent.enabled = true;
                agent.isStopped = false;
                agent.ResetPath();
                break;
        }
    }

    private int ChoseNewTrainingSpot()
    {
        if (trainingSpots.Length == 1) return 0;

        int newSpotIndex;
        do
        {
            newSpotIndex = Random.Range(0, trainingSpots.Length);
        } while (newSpotIndex == lastSpotIndex);

        lastSpotIndex = newSpotIndex;

        return newSpotIndex;
    }

    private void MoveToSpot()
    {
        if (agent.pathPending) return;
        if (agent.hasPath && agent.remainingDistance > 0.5f) return;

        int spotIndex = ChoseNewTrainingSpot();
        if (spotIndex < 0) return;

        targetSpot = trainingSpots[spotIndex];

        agent.SetDestination(targetSpot.position);
    }

    private void SwitchFightColliders(string onOff)
    {
        switch (onOff)
        {
            case "On":
                foreach (Collider collider in fightColliders)
                {
                    collider.enabled = true;
                }
                break;

            case "Off":
                foreach (Collider collider in fightColliders)
                {
                    collider.enabled = false;
                }
                break;
        }
    }

    private bool NeedToFlee()
    {
        if (!wasBeaten) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);

        if (distanceToPlayer < fleeDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandlePatrol()
    {
        MoveToSpot();
        UpdateWalkingPhase();
    }

    private void UpdateWalkingPhase()
    {
        CurrentWalkingPhase = agent.velocity.magnitude > 0.1f ? WalkingPhase.Walking : WalkingPhase.Idle;
    }


    private void HandleChasing()
    {
        agent.SetDestination(playerController.transform.position);
    }

    private void HandleFleeing()
    {
        Vector3 dirAway = (transform.position - playerController.transform.position).normalized;
        Vector3 target = transform.position + dirAway * 6f;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void HandleTraining()
    {

    }

    private void HandleFighting()
    {

    }

    private bool CanChase()
    {
        if (playerController == null) return false;
        if (wasBeaten) return false;

        Vector3 toPlayer = playerController.transform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > awarenessDistance) return false;

        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z);

        if (flatToPlayer.sqrMagnitude < 0.001f) return true;

        float angleToPlayer = Vector3.Angle(flatForward, flatToPlayer);

        return angleToPlayer <= awarenessAngle * 0.5f;
    }

    public void StartTraining(TrainingData trainingData)
    {
        currentTrainingData = trainingData;
        ChangeState(State.Training);
    }

    public void StopTraining()
    {
        if (currentTrainingData != null)
        {
            transform.position = currentTrainingData.exitPos.position;
            transform.rotation = currentTrainingData.exitPos.rotation;
        }

        currentTrainingData = null;
        ChangeState(State.Patrol);
    }

    public void CancelTraining()
    {
        agent.ResetPath();
    }
}