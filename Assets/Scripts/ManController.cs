using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour, IAgent
{
    // FIELDS //////////////////////////////////////////////////////////////////////


    // Refs ________________________________________________________________________

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    public Animator animator;


    // General ________________________________________________________________________

    [SerializeField] private int agentPriority;


    // State __________________________________________________________________________

    public enum State { Patrol, Chasing, Fleeing, Training, Fighting }
    public State CurrentState { get; private set; } = State.Patrol;


    // Patrol  ________________________________________________________________________

    public enum WalkingPhase { None, Idle, Walking };
    public WalkingPhase CurrentWalkingPhase { get; private set; }
    [SerializeField] Transform[] trainingSpots;
    private Transform targetSpot;
    private int lastSpotIndex = -1;


    // Chasing  ______________________________________________________________________

    private float chasingDistance = 4f;
    private float chasingAngle = 220f;


    // Fleeing  ______________________________________________________________________

    private float fleeDistance = 6f;
    public bool wasBeaten;


    // Insult  ________________________________________________________________________

    private float insultDistance = 1.5f;
    private bool hasInsulted;


    // Training  ______________________________________________________________________

    private TrainingData currentTrainingData;
    public ManTrainingType CurrentTrainingType { get; private set; }


    // Fighting  ______________________________________________________________________

    [SerializeField] Collider[] fightColliders;


    public enum FightPhase { None, Attack, Blocked, Victory, Defeat }
    public FightPhase CurrentFightPhase { get; private set; }


    public enum FightSide { None, Left, Right }
    public FightSide CurrentFightSide { get; private set; }


    public enum FallDirection { Front, Back, Left, Right }
    public FallDirection CurrentFallDirection { get; private set; }


    public enum ManFightAction //_______________ animation interface
    {
        None = 0, Idle = 1, AttackLeft = 2, AttackRight = 3, BlockedLeft = 4,
        BlockedRight = 5, FallBack = 6, FallFront = 7, FallLeft = 8, FallRight = 9, Victory = 10
    }
    public ManFightAction CurrentFightAction { get; private set; } //_______________ animation interface


    public bool IsFightResolved { get; private set; }
    private float nextAttackTimer = 0.5f;


    // LIFECYCLE //////////////////////////////////////////////////////////////////////

    void Awake()
    {
        agent.avoidancePriority = agentPriority;
        SwitchFightColliders(false);
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

    // HANDLERS //////////////////////////////////////////////////////////////////////

    private void HandlePatrol()
    {
        MoveToSpot();
        UpdateWalkingPhase();
    }
    private void HandleChasing()
    {
        agent.SetDestination(playerController.transform.position);
        UpdateWalkingPhase();
        BeReadyToInsult();
    }

    private void HandleFleeing()
    {
        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 target = transform.position + dirAway * 6f;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        UpdateWalkingPhase();
        BeReadyToInsult();
    }

    private void HandleTraining()
    {

    }

    private void HandleFighting()
    {
        SwitchFightActions();

        if (IsFightResolved) return;

        if (CurrentFightPhase != FightPhase.None) return;

        nextAttackTimer -= Time.deltaTime;
        if (nextAttackTimer > 0f) return;

        Attack();
    }

    // TRANSITIONS /////////////////////////////////////////////////////////////////////

    public void ChangeState(State nextState)
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
                SwitchFightColliders(false);
                ChangeFightPhase(FightPhase.None);
                ChangeFightSide(FightSide.None);
                break;
            case State.Patrol:
                ChangeWalkingPhase(WalkingPhase.None);
                break;
            case State.Fleeing:
                ChangeWalkingPhase(WalkingPhase.None);
                break;
            case State.Chasing:
                ChangeWalkingPhase(WalkingPhase.None);
                break;
        }
    }

    private void EnterState(State state)
    {
        switch (state)
        {
            case State.Patrol:
                SwitchFightColliders(false);
                ChangeFightPhase(FightPhase.None);
                ChangeFightSide(FightSide.None);
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
                hasInsulted = false;
                agent.isStopped = true;
                agent.ResetPath();
                agent.enabled = false;
                transform.position = currentTrainingData.trainingPos.position;
                transform.rotation = currentTrainingData.trainingPos.rotation;
                break;

            case State.Fighting:
                hasInsulted = false;
                agent.ResetPath();
                agent.isStopped = true;
                agent.enabled = false;
                transform.LookAt(player);
                SwitchFightColliders(true);
                Attack();
                break;

            case State.Fleeing:
                agent.enabled = true;
                agent.isStopped = false;
                agent.ResetPath();
                break;
        }
    }


    // HELPERS ///////////////////////////////////////////////////////////////////////////

    // Patrol  ___________________________________________________________________________

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

    private void MoveToSpot()
    {
        if (agent.pathPending) return;
        if (agent.hasPath && agent.remainingDistance > 0.5f) return;

        int spotIndex = ChoseNewTrainingSpot();
        if (spotIndex < 0) return;

        targetSpot = trainingSpots[spotIndex];

        agent.SetDestination(targetSpot.position);
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


    // Patrol <-> Chasing  _________________________________________________________________

    private bool CanChase()
    {
        if (player == null) return false;
        if (wasBeaten) return false;

        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > chasingDistance) return false;

        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z);

        if (flatToPlayer.sqrMagnitude < 0.001f) return true;

        float angleToPlayer = Vector3.Angle(flatForward, flatToPlayer);

        return angleToPlayer <= chasingAngle * 0.5f;
    }


    // Patrol <-> Fleeing  _________________________________________________________________

    private bool NeedToFlee()
    {
        if (!wasBeaten) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fleeDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // Chasing, Fleeing -> Insult  __________________________________________________________

    private void BeReadyToInsult()
    {
        if (GameManager.Instance.CurrentScore < 1) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (CurrentState == State.Fleeing && distanceToPlayer > insultDistance + 1f) hasInsulted = false;
        if (distanceToPlayer > insultDistance) return;

        if (hasInsulted) return;
        hasInsulted = true;

        Debug.Log("You little nerd!");
        GameManager.Instance.ModifyScore(-1);
    }


    // Fighting  _____________________________________________________________________________

    private void SwitchFightColliders(bool isEnabled)
    {
        foreach (Collider collider in fightColliders)
        {
            collider.enabled = isEnabled;
            collider.gameObject.SetActive(isEnabled);
        }
    }

    public void Attack()
    {
        int fightSide = Random.Range(1, 3);

        switch (fightSide)
        {
            case 1:
                ChangeFightSide(FightSide.Left);
                break;
            case 2:
                ChangeFightSide(FightSide.Right);
                break;
        }

        ChangeFightPhase(FightPhase.Attack);
    }

    public void ChangeFightPhase(FightPhase nextPhase)
    {
        if (CurrentFightPhase == nextPhase) return;
        CurrentFightPhase = nextPhase;
    }

    private void ChangeFightSide(FightSide nextSide)
    {
        if (CurrentFightSide == nextSide) return;
        CurrentFightSide = nextSide;
    }

    public void ChangeFallDirection(FallDirection nextDirection)
    {
        if (CurrentFallDirection == nextDirection) return;
        CurrentFallDirection = nextDirection;
    }

    public void OnBlockedAnimationFinished() //_______________ animation event via Relay
    {
        if (CurrentState != State.Fighting) return;
        if (CurrentFightPhase != FightPhase.Blocked) return;
        ChangeFightPhase(FightPhase.None);
        ChangeFightSide(FightSide.None);
        nextAttackTimer = 0.5f;
    }

    public void OnDefeatAnimationFinished() //_______________ animation event via Relay
    {
        if (CurrentState != State.Fighting) return;
        if (CurrentFightPhase != FightPhase.Defeat) return;

        wasBeaten = true;
        ChangeState(State.Fleeing);
    }

    public void SetFightResolved(bool value) //_______________ called by FightZone
    {
        IsFightResolved = value;
    }

    private void SwitchFightActions() //_______________ animation interface
    {
        switch (CurrentFightPhase)
        {
            case FightPhase.None:
                CurrentFightAction = ManFightAction.Idle;
                break;
            case FightPhase.Attack:
                switch (CurrentFightSide)
                {
                    case FightSide.Left:
                        CurrentFightAction = ManFightAction.AttackLeft;
                        break;
                    case FightSide.Right:
                        CurrentFightAction = ManFightAction.AttackRight;
                        break;
                }
                break;
            case FightPhase.Blocked:
                switch (CurrentFightSide)
                {
                    case FightSide.Left:
                        CurrentFightAction = ManFightAction.BlockedLeft;
                        break;
                    case FightSide.Right:
                        CurrentFightAction = ManFightAction.BlockedRight;
                        break;
                }
                break;
            case FightPhase.Victory:
                CurrentFightAction = ManFightAction.Victory;
                break;
            case FightPhase.Defeat:
                switch (CurrentFallDirection)
                {
                    case FallDirection.Back:
                        CurrentFightAction = ManFightAction.FallBack;
                        break;
                    case FallDirection.Front:
                        CurrentFightAction = ManFightAction.FallFront;
                        break;
                    case FallDirection.Left:
                        CurrentFightAction = ManFightAction.FallLeft;
                        break;
                    case FallDirection.Right:
                        CurrentFightAction = ManFightAction.FallRight;
                        break;
                }
                break;
        }
    }

    // Training  ____________________________________________________________________________

    public void StartTraining(TrainingData trainingData)
    {
        currentTrainingData = trainingData;
        CurrentTrainingType = currentTrainingData.manTrainingType;
        ChangeState(State.Training);
    }

    public void StopTraining()
    {
        if (currentTrainingData != null)
        {
            transform.position = currentTrainingData.exitPos.position;
            transform.rotation = currentTrainingData.exitPos.rotation;
            currentTrainingData = null;
        }

        CurrentTrainingType = ManTrainingType.None;
        ChangeState(State.Patrol);
    }
}
