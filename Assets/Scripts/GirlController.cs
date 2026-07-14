using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;

public class GirlController : MonoBehaviour, IAgent
{
    private enum SceneMode { Gameplay, YouWin }

    public enum State { Patrol, Chasing, Fleeing, Training, Welcoming }
    public enum WalkingPhase { None, Idle, Walking };

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] trainingSpots;
    private SceneMode sceneMode = SceneMode.Gameplay;

    private TrainingData currentTrainingData;

    public WalkingPhase CurrentWalkingPhase { get; private set; }
    public GirlTrainingType CurrentTrainingType { get; private set; } // pour les variantes d'animation sur DumbbellsStand

    private Transform targetSpot;
    private int lastSpotIndex = -1;
    private float awarenessDistance = 3f;
    private float interactionDistance = 2.5f;
    private float fleeStopDistance = 4f;
    private bool hasInteracted;

    public State CurrentState { get; private set; } = State.Patrol;

    [SerializeField] private int agentPriority;

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

        agent.avoidancePriority = agentPriority;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case State.Patrol:
                HandlePatrol();
                break;

            case State.Fleeing:
                // HandleFleeing();
                break;

            case State.Welcoming:
                // HandleWelcoming();
                break;

            case State.Training:
                HandleTraining();
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
            case State.Patrol:
                ChangeWalkingPhase(WalkingPhase.None);
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
                hasInteracted = false;
                agent.isStopped = true;
                agent.ResetPath();
                agent.enabled = false;
                transform.position = currentTrainingData.trainingPos.position;
                transform.rotation = currentTrainingData.trainingPos.rotation;
                break;

            case State.Fleeing:
                agent.enabled = true;
                agent.isStopped = false;
                agent.ResetPath();
                break;
        }
    }

    private void UpdateWalkingPhase()
    {
        WalkingPhase nextPhase = agent.velocity.magnitude > 0.1f ? WalkingPhase.Walking : WalkingPhase.Idle;
        ChangeWalkingPhase(nextPhase);
    }

    private void ChangeWalkingPhase(WalkingPhase nextPhase)
    {
        if (CurrentWalkingPhase == nextPhase) return;
        CurrentWalkingPhase = nextPhase;
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

    public void CancelTraining()
    {
        agent.ResetPath();
    }

    private void HandlePatrol()
    {
        MoveToSpot();
        UpdateWalkingPhase();
    }

    private void HandleTraining()
    {

    }

    // private void HandleFleeing()
    // {
    //     float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    //     if (distanceToPlayer < interactionDistance)
    //     {
    //         StartCoroutine(DoInsult("Fuck off loser!", -1));
    //         return;
    //     }

    //     if (distanceToPlayer > fleeStopDistance)
    //     {
    //         currentState = State.MovingToTarget;
    //         agent.ResetPath();
    //         return;
    //     }

    //     Vector3 dirAway = (transform.position - player.position).normalized;
    //     Vector3 target = transform.position + dirAway * 6f;

    //     if (NavMesh.SamplePosition(target, out NavMeshHit hit, 8f, NavMesh.AllAreas))
    //     {
    //         agent.SetDestination(hit.position);
    //     }
    // }

    // private void HandleWelcoming()
    // {
    //     float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    //     if (distanceToPlayer < interactionDistance)
    //     {
    //         StartCoroutine(DoWelcome("Hello honey!"));
    //         return;
    //     }

    //     if (distanceToPlayer > welcomingStopDistance)
    //     {
    //         currentState = State.MovingToTarget;
    //         agent.ResetPath();
    //         return;
    //     }

    //     agent.SetDestination(player.position);
    // }

    public void StartTraining(TrainingData trainingData)
    {
        StartTraining(trainingData, trainingData.girlTrainingType);
    }

    public void StartTraining(TrainingData trainingData, GirlTrainingType girlTrainingType)
    {
        CurrentTrainingType = girlTrainingType;
        currentTrainingData = trainingData;
        ChangeState(State.Training);
    }

    public void StopTraining()
    {
        transform.position = currentTrainingData.exitPos.position;
        transform.rotation = currentTrainingData.exitPos.rotation;

        currentTrainingData = null;
        CurrentTrainingType = GirlTrainingType.None;

        ChangeState(State.Patrol);
    }

    // private IEnumerator DoInsult(string message, int scoreDelta)
    // {
    //     if (hasInteracted) yield break;

    //     hasInteracted = true;
    //     isPerformingInteraction = true;
    //     agent.ResetPath();
    //     agent.isStopped = true;
    //     Debug.Log(message);
    //     GameManager.Instance.ModifyScore(scoreDelta);
    //     yield return new WaitForSeconds(0.1f);
    //     agent.isStopped = false;
    //     isPerformingInteraction = false;
    //     currentState = State.MovingToTarget;
    // }

    // private IEnumerator DoWelcome(string message)
    // {
    //     if (hasInteracted) yield break;

    //     hasInteracted = true;
    //     isPerformingInteraction = true;
    //     agent.ResetPath();
    //     agent.isStopped = true;
    //     Debug.Log(message);
    //     // Hook for a future DoubleSelfieZone / cutscene flow in YouWin.
    //     yield return new WaitForSeconds(0.1f);
    //     agent.isStopped = false;
    //     isPerformingInteraction = false;
    //     currentState = State.MovingToTarget;
    // }
}
