using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private Animator animator;
    public Transform playerCameraTarget;
    public Transform playerCameraPlace;
    [SerializeField] private GameObject stopTrainingControl;
    public Transform entryPoint;
    private Vector2 playerMovement;
    private Vector2 mouseDelta;

    public TrainingData CurrentTrainingData { get; private set; }
    private IPlayerTrainingHost currentTrainingHost;

    public FightZone CurrentFightZone { get; private set; }

    public bool playerFightAttack;
    public bool playerFightLeft;
    public bool playerFightRight;
    public bool isThrowing;

    public JumpZone CurrentJumpZone { get; private set; }
    [SerializeField] Collider[] fightColliders;

    private float landedTimer;
    public float JumpChargeCoeff { get; private set; }
    public float JumpChargeBounce { get; private set; }
    private float jumpChargeBounceTime;
    private float jumpChargeBounceSpeed = 1f;

    public Door CurrentDoor { get; private set; }
    private float doorTimer;

    public Pole CurrentPole { get; private set; }
    float fullClimbingHeight = 20f;
    float climbingHeightLimit;

    public enum State { Walking, Gaming, Training, Fighting, Falling, Jumping, PushingTheDoor, ClimbingThePole, MakingDoubleSelfie };
    public enum WalkingPhase { None, Idle, Walking };
    public enum JumpPhase { None, Charging, Squatting, Released, Airborne, Landed };
    public enum DoorPhase { None, Pushing, Releasing }
    public enum ClimbPhase { None, ClimbingUp, SlidingDown }
    public enum FightPhase { None, Block, Attack, Victory, Defeat }
    public enum FallDirection { Front, Back, Left, Right }
    public enum FightSide { None, Left, Right, Front }
    public enum PlayerFightAction
    {
        None = 0,
        Idle = 1,
        AttackFront = 2,
        AttackLeft = 3,
        AttackRight = 4,
        BlockLeft = 5,
        BlockRight = 6,
        FallBack = 7,
        FallFront = 8,
        FallLeft = 9,
        FallRight = 10,
        Victory = 11
    }

    public State CurrentState { get; private set; } = State.Walking;
    public WalkingPhase CurrentWalkingPhase { get; private set; }
    public JumpPhase CurrentJumpPhase { get; private set; }
    public DoorPhase CurrentDoorPhase { get; private set; }
    public ClimbPhase CurrentClimbPhase { get; private set; }
    public PlayerTrainingType CurrentTrainingType { get; private set; }
    public FightPhase CurrentFightPhase { get; private set; }
    public FallDirection CurrentFallDirection { get; private set; }
    public FightSide CurrentFightSide { get; private set; }
    public PlayerFightAction CurrentFightAction { get; private set; }

    [SerializeField] private LayerMask groundMask;

    public event Action OpenTheDoor;

    void Awake()
    {
        SwitchFightColliders(false);
    }

    void FixedUpdate()
    {
        switch (CurrentState)
        {
            case State.Walking:
                HandleWalking();
                break;
            case State.Training:
                HandleTraining();
                break;
            case State.Fighting:
                HandleFighting();
                break;
            case State.Falling:
                HandleFalling();
                break;
            case State.Jumping:
                HandleJumping();
                break;
            case State.PushingTheDoor:
                HandlePushingTheDoor();
                break;
            case State.ClimbingThePole:
                HandleClimbingThePole();
                break;
            case State.MakingDoubleSelfie:
                HandleMakingDoubleSelfie();
                break;
        }

        SwitchPlayerLayer();
    }

    public void ChangeState(State nextState)
    {
        if (CurrentState == nextState) return;

        ResetSubStatesLeavingState(CurrentState);
        CurrentState = nextState;
        SetKinematicProperty(CurrentState);

        if (CurrentState == State.Walking)
        {
            ReinitCameraPlace();
            SwitchFightColliders(false);
        }
    }

    private void ReinitCameraPlace()
    {
        playerCameraPlace.localPosition = new Vector3(0, 1.9f, -1f);
    }

    private void SetKinematicProperty(State state)
    {
        switch (state)
        {
            case State.Walking:
                rb.isKinematic = false;
                break;
            case State.Training:
                rb.isKinematic = true;
                break;
            case State.Fighting:
                rb.isKinematic = false;
                break;
            case State.Falling:
                rb.isKinematic = false;
                break;
            case State.Jumping:
                rb.isKinematic = false;
                break;
            case State.PushingTheDoor:
                rb.isKinematic = true;
                break;
            case State.ClimbingThePole:
                rb.isKinematic = true;
                break;
            case State.MakingDoubleSelfie:
                rb.isKinematic = false;
                break;
        }
    }

    private void ChangeWalkingPhase(WalkingPhase nextPhase)
    {
        if (CurrentWalkingPhase == nextPhase) return;

        CurrentWalkingPhase = nextPhase;
    }

    private void ChangeJumpPhase(JumpPhase nextPhase)
    {
        if (CurrentJumpPhase == nextPhase) return;

        CurrentJumpPhase = nextPhase;
    }

    private void ChangeDoorPhase(DoorPhase nextPhase)
    {
        if (CurrentDoorPhase == nextPhase) return;

        CurrentDoorPhase = nextPhase;
    }

    private void ChangeClimbPhase(ClimbPhase nextPhase)
    {
        if (CurrentClimbPhase == nextPhase) return;

        CurrentClimbPhase = nextPhase;
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


    private void SetTrainingType(PlayerTrainingType type)
    {
        if (CurrentTrainingType == type) return;

        CurrentTrainingType = type;
    }

    private void ResetSubStatesLeavingState(State state)
    {
        switch (state)
        {
            case State.Walking:
                ChangeWalkingPhase(WalkingPhase.None);
                break;

            case State.Jumping:
                ChangeJumpPhase(JumpPhase.None);
                break;

            case State.PushingTheDoor:
                ChangeDoorPhase(DoorPhase.None);
                break;

            case State.ClimbingThePole:
                ChangeClimbPhase(ClimbPhase.None);
                break;

            case State.Fighting:
                ChangeFightPhase(FightPhase.None);
                ChangeFightSide(FightSide.None);
                SwitchFightColliders(false);
                isThrowing = false;
                break;
        }
    }

    private void HandleWalking()
    {
        MovePlayer();
        RotatePlayer();
        MoveCameraTarget();
        ChangeWalkingPhase(CheckIfWalking() ? WalkingPhase.Walking : WalkingPhase.Idle);
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        stopTrainingControl.SetActive(false);
    }

    private void HandleTraining()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        stopTrainingControl.SetActive(true);
    }

    private void HandleFighting()
    {
        RotatePlayer();
        MoveCameraTarget();
        ProcessFightInputs();
        SwitchFightActions();
    }

    private void ProcessFightInputs()
    {
        if (CurrentFightPhase == FightPhase.Victory || CurrentFightPhase == FightPhase.Defeat)
            return;

        if (isThrowing)
            return;

        if (GameManager.Instance.BackTraining >= 0.5f)
        {
            if (playerFightLeft)
            {
                if (playerFightAttack)
                {
                    playerFightAttack = false;
                    ChangeFightPhase(FightPhase.Attack);
                    ChangeFightSide(FightSide.Left);
                    isThrowing = true;
                }
                else
                {
                    ChangeFightPhase(FightPhase.Block);
                    ChangeFightSide(FightSide.Left);
                    isThrowing = true;
                }
                return;
            }

            if (playerFightRight)
            {
                if (playerFightAttack)
                {
                    playerFightAttack = false;
                    ChangeFightPhase(FightPhase.Attack);
                    ChangeFightSide(FightSide.Right);
                    isThrowing = true;
                }
                else
                {
                    ChangeFightPhase(FightPhase.Block);
                    ChangeFightSide(FightSide.Right);
                    isThrowing = true;
                }
                return;
            }
        }

        if (GameManager.Instance.ChestTraining >= 0.5f && playerFightAttack)
        {
            playerFightAttack = false;
            ChangeFightPhase(FightPhase.Attack);
            ChangeFightSide(FightSide.Front);
            isThrowing = true;
            return;
        }

        playerFightAttack = false;
        ChangeFightPhase(FightPhase.None);
        ChangeFightSide(FightSide.None);
    }

    private void SwitchFightActions()
    {
        switch (CurrentFightPhase)
        {
            case FightPhase.None:
                CurrentFightAction = PlayerFightAction.Idle;
                break;
            case FightPhase.Block:
                switch (CurrentFightSide)
                {
                    case FightSide.Left:
                        CurrentFightAction = PlayerFightAction.BlockLeft;
                        break;
                    case FightSide.Right:
                        CurrentFightAction = PlayerFightAction.BlockRight;
                        break;
                }
                break;
            case FightPhase.Attack:
                switch (CurrentFightSide)
                {
                    case FightSide.Front:
                        CurrentFightAction = PlayerFightAction.AttackFront;
                        break;
                    case FightSide.Left:
                        CurrentFightAction = PlayerFightAction.AttackLeft;
                        break;
                    case FightSide.Right:
                        CurrentFightAction = PlayerFightAction.AttackRight;
                        break;
                }
                break;
            case FightPhase.Victory:
                CurrentFightAction = PlayerFightAction.Victory;
                break;
            case FightPhase.Defeat:
                switch (CurrentFallDirection)
                {
                    case FallDirection.Back:
                        CurrentFightAction = PlayerFightAction.FallBack;
                        break;
                    case FallDirection.Front:
                        CurrentFightAction = PlayerFightAction.FallFront;
                        break;
                    case FallDirection.Left:
                        CurrentFightAction = PlayerFightAction.FallLeft;
                        break;
                    case FallDirection.Right:
                        CurrentFightAction = PlayerFightAction.FallRight;
                        break;
                }
                break;
        }
    }

    public void OnThrowFinished()
    {
        isThrowing = false;
        ChangeFightPhase(FightPhase.None);
        ChangeFightSide(FightSide.None);
    }

    public void OnDefeatAnimationFinished()
    {
        if (CurrentState != State.Fighting) return;
        if (CurrentFightPhase != FightPhase.Defeat) return;

        GameManager.Instance.RequestDeath(GameManager.DeathReason.Fight);
    }

    private void AbortCurrentContextForDeath()
    {
        Cursor.visible = false;
        stopTrainingControl.SetActive(false);

        if (CurrentTrainingData != null)
        {
            currentTrainingHost?.ReleaseTrainingSpot();
            currentTrainingHost = null;
            CurrentTrainingData = null;
            SetTrainingType(PlayerTrainingType.None);
        }

        CurrentDoor = null;
        CurrentPole = null;
        climbingHeightLimit = 0f;
        CurrentJumpZone = null;

        ChangeWalkingPhase(WalkingPhase.None);
        ChangeJumpPhase(JumpPhase.None);
        ChangeDoorPhase(DoorPhase.None);
        ChangeClimbPhase(ClimbPhase.None);

        CurrentFightZone = null;
        playerFightAttack = false;
        playerFightLeft = false;
        playerFightRight = false;
        isThrowing = false;
        CurrentFightPhase = FightPhase.None;
        CurrentFightSide = FightSide.None;
        CurrentFightAction = PlayerFightAction.None;
        SwitchFightColliders(false);
    }

    private void HandleJumping()
    {
        switch (CurrentJumpPhase)
        {
            case JumpPhase.Charging:
                HandleJumpCharging();
                break;

            case JumpPhase.Squatting:
                HandleJumpSquatting();
                break;

            case JumpPhase.Released:
                HandleJumpReleased();
                break;

            case JumpPhase.Airborne:
                HandleJumpAirborne();
                break;

            case JumpPhase.Landed:
                HandleJumpLanded();
                break;
        }
    }

    private void HandleJumpCharging()
    {
        RotatePlayer();
        MoveCameraTarget();
        ChargeJump();
        ChargeBounce();
        BouncePlayer();
    }

    private void HandleJumpSquatting()
    {
        RotatePlayer();
        MoveCameraTarget();
    }

    private void HandleJumpReleased()
    {
        rb.angularVelocity = Vector3.zero;

        if (!CheckIfIsGrounded())
            ChangeJumpPhase(JumpPhase.Airborne);
    }

    private void HandleJumpAirborne()
    {
        rb.angularVelocity = Vector3.zero;

        if (CurrentJumpPhase != JumpPhase.Airborne) return;
        if (CheckIfIsGrounded())
        {
            landedTimer = 0.5f;
            ChangeJumpPhase(JumpPhase.Landed);
        }
    }

    private void SwitchPlayerLayer()
    {
        switch (CurrentState)
        {
            case State.Jumping:
                string layerName = CheckIfIsGrounded() ? "Player" : "PlayerAirborne";
                gameObject.layer = LayerMask.NameToLayer(layerName);
                break;
            default:
                gameObject.layer = LayerMask.NameToLayer("Player");
                break;
        }
    }

    private void HandleJumpLanded()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        landedTimer -= Time.fixedDeltaTime;

        if (landedTimer > 0f) return;

        ChangeJumpPhase(JumpPhase.None);
        ChangeState(State.Walking);
    }

    private bool CheckIfIsGrounded()
    {
        return Physics.CheckSphere(rb.position, 0.2f, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void HandlePushingTheDoor()
    {
        if (CurrentDoorPhase == DoorPhase.Releasing) WaitAndWalk();

        rb.position = CurrentDoor.pushingPos.position;
        rb.rotation = CurrentDoor.pushingPos.rotation;
    }

    private void WaitAndWalk()
    {
        doorTimer -= Time.fixedDeltaTime;
        if (doorTimer > 0) return;
        ChangeState(State.Walking);
    }

    private void HandleClimbingThePole()
    {
        if (CurrentPole == null)
        {
            ChangeState(State.Walking);
            return;
        }

        MoveCameraTarget();

        float climbingMaxHeight = CurrentPole.transform.position.y + climbingHeightLimit;

        switch (CurrentClimbPhase)
        {
            case ClimbPhase.ClimbingUp:
                rb.MovePosition(rb.position + Vector3.up * Time.fixedDeltaTime * 2);
                break;
            case ClimbPhase.SlidingDown:
                rb.MovePosition(rb.position - Vector3.up * Time.fixedDeltaTime * 4);
                break;
        }

        if (CheckIfIsGrounded()) ChangeState(State.Walking);
        if (transform.position.y >= climbingMaxHeight) ChangeClimbPhase(ClimbPhase.SlidingDown);
    }

    private void HandleFalling()
    {
        FreezeCameraHeight();
    }

    private void FreezeCameraHeight()
    {
        float frozenCameraHeight = playerCameraPlace.position.y;
        playerCameraPlace.position = new Vector3(transform.position.x, frozenCameraHeight, transform.position.z);
    }

    public void RespawnAtEntryPoint()
    {
        AbortCurrentContextForDeath();
        ReinitCameraPlace();


        ChangeState(State.Walking);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = entryPoint.position;
        rb.rotation = entryPoint.rotation;
    }

    private void HandleMakingDoubleSelfie()
    {

    }

    private void MovePlayer()
    {
        float moveSpeed = 1.5f;
        Vector2 movementInput = playerMovement.normalized;

        Vector3 horizontalVelocity = transform.forward * movementInput.y * moveSpeed + transform.right * movementInput.x * moveSpeed;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void RotatePlayer()
    {
        float yawDelta = mouseDelta.x * 0.3f;
        rb.angularVelocity = new Vector3(0, yawDelta, 0);
    }

    private void MoveCameraTarget()
    {
        float pitchDelta = mouseDelta.y * 0.5f;
        pitchDelta = Mathf.Clamp(pitchDelta, -1f, 1.5f);
        float pitch = playerCameraTarget.localPosition.y + pitchDelta * 1.5f * Time.fixedDeltaTime;

        playerCameraTarget.localPosition = new Vector3(0, pitch, 0);
    }

    private bool CheckIfWalking()
    {
        return Mathf.Abs(playerMovement.x) > 0.1f || Mathf.Abs(playerMovement.y) > 0.1f;
    }

    public void EnterFightZone(FightZone fightZone)
    {
        CurrentFightZone = fightZone;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ChangeState(State.Fighting);
        ChangeFightPhase(FightPhase.None);
        ChangeFightSide(FightSide.None);
        SwitchFightColliders(true);
    }

    public void ExitFightZone()
    {
        CurrentFightZone = null;
        ChangeState(State.Walking);
        SwitchFightColliders(false);
    }

    private void SwitchFightColliders(bool isEnabled)
    {
        foreach (Collider collider in fightColliders)
        {
            collider.enabled = isEnabled;
            collider.gameObject.SetActive(isEnabled);
        }
    }

    public void EnterJumpZone(JumpZone jumpZone)
    {
        CurrentJumpZone = jumpZone;
        if (jumpZone.jumpingPos) transform.position = CurrentJumpZone.jumpingPos.position;
    }

    public void ExitJumpZone(JumpZone jumpZone)
    {
        if (CurrentJumpZone != jumpZone) return;
        CurrentJumpZone = null;
    }

    private void ChargeJump()
    {
        if (CurrentJumpZone == null) return;
        JumpChargeCoeff += Time.fixedDeltaTime / 6;
        JumpChargeCoeff = Mathf.Clamp(JumpChargeCoeff, 0f, 1f);
    }

    private void ChargeBounce()
    {
        jumpChargeBounceTime += Time.fixedDeltaTime * jumpChargeBounceSpeed;
        JumpChargeBounce = 0.5f + 0.5f * Mathf.Sin(jumpChargeBounceTime * Mathf.PI * 2f);
    }

    private void BouncePlayer()
    {
        if (CurrentJumpZone == null) return;

        Vector3 targetPos = Vector3.Lerp(CurrentJumpZone.chargingLowPos.position, CurrentJumpZone.chargingHighPos.position, JumpChargeBounce);

        rb.linearVelocity = Vector3.zero;
        rb.MovePosition(targetPos);
    }

    private void StartChargingJump()
    {
        JumpChargeCoeff = 0f;
        jumpChargeBounceTime = 0f;
        JumpChargeBounce = 0f;

        ChangeState(State.Jumping);

        if (CurrentJumpZone.jumpType == JumpZone.JumpType.Charged)
        {
            ChangeJumpPhase(JumpPhase.Charging);
            rb.position = CurrentJumpZone.jumpingPos.position;
        }
        else
        {
            ChangeJumpPhase(JumpPhase.Squatting);
        }
    }

    private void ReleaseJump()
    {
        if (CurrentJumpPhase != JumpPhase.Charging && CurrentJumpPhase != JumpPhase.Squatting) return;

        ChangeJumpPhase(JumpPhase.Released);

        switch (CurrentJumpZone.jumpType)
        {
            case JumpZone.JumpType.Plain:
                rb.linearVelocity = transform.forward * 7.5f + transform.up * 2;
                break;

            case JumpZone.JumpType.Charged:
                float legsTraining = GameManager.Instance.LegsTraining;

                float verticalCharge = Mathf.Sqrt(JumpChargeCoeff);
                float horizontalCharge = JumpChargeCoeff * JumpChargeCoeff;

                float minVerticalForce = 1.5f;
                float maxVerticalForceLowTraining = 7f;
                float maxVerticalForceFullTraining = 20f;

                float verticalForceLowTraining = Mathf.Lerp(minVerticalForce, maxVerticalForceLowTraining, verticalCharge);
                float verticalForceFullTraining = Mathf.Lerp(minVerticalForce, maxVerticalForceFullTraining, verticalCharge);
                float verticalForce = Mathf.Lerp(verticalForceLowTraining, verticalForceFullTraining, legsTraining);

                float horizontalForce = 0f;

                if (legsTraining >= 1f)
                {
                    horizontalForce = Mathf.Lerp(0f, 15f, horizontalCharge);
                }

                rb.linearVelocity = transform.forward * horizontalForce + transform.up * verticalForce;
                break;
        }
    }

    public void EnterDoorZone(Door door)
    {
        CurrentDoor = door;
    }

    public void ExitDoorZone()
    {
        CurrentDoor = null;
        ChangeDoorPhase(DoorPhase.None);
        ChangeState(State.Walking);
    }

    private void StartPushing()
    {
        ChangeState(State.PushingTheDoor);
        ChangeDoorPhase(DoorPhase.Pushing);
    }

    private void ReleaseTheDoor()
    {
        if (GameManager.Instance.ChestTraining < 1f)
        {
            ChangeDoorPhase(DoorPhase.None);
            ChangeState(State.Walking);
        }
        else
        {
            ChangeDoorPhase(DoorPhase.Releasing);
            OpenTheDoor?.Invoke();
            doorTimer = 0.3f;
        }
    }

    public void EnterClimbZone(Pole pole)
    {
        CurrentPole = pole;
        climbingHeightLimit = fullClimbingHeight * GameManager.Instance.BackTraining;
    }

    public void ExitClimbZone()
    {
        CurrentPole = null;
        climbingHeightLimit = 0f;
    }

    private void StartClimbing()
    {
        transform.position = CurrentPole.climbingPos.position;
        ChangeState(State.ClimbingThePole);
        ChangeClimbPhase(ClimbPhase.ClimbingUp);
    }

    private void ReleaseClimbing()
    {
        ChangeClimbPhase(ClimbPhase.SlidingDown);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (CurrentFightZone != null) return;
        playerMovement = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (CurrentFightZone == null) return;
        if (isThrowing) return;

        if (ctx.started)
        {
            playerFightAttack = true;
        }
    }

    public void OnAttackSideLeft(InputAction.CallbackContext ctx)
    {
        // if (CurrentFightZone == null) return;
        // if (isThrowing) return;

        // if (ctx.started)
        // {
        //     playerFightLeft = true;
        // }

        if (CurrentFightZone == null) return;

        playerFightLeft = ctx.ReadValueAsButton();
    }

    public void OnAttackSideRight(InputAction.CallbackContext ctx)
    {
        if (CurrentFightZone == null) return;

        playerFightRight = ctx.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (CurrentJumpZone == null) return;

        if (ctx.started)
        {
            StartChargingJump();
        }

        if (ctx.canceled)
        {
            ReleaseJump();
        }
    }

    public void OnPush(InputAction.CallbackContext ctx)
    {
        if (CurrentDoor == null) return;

        if (ctx.started)
        {
            StartPushing();
        }

        if (ctx.canceled)
        {
            ReleaseTheDoor();
        }
    }

    public void OnClimb(InputAction.CallbackContext ctx)
    {
        if (CurrentPole == null) return;

        if (ctx.started)
        {
            StartClimbing();
        }

        if (ctx.canceled)
        {
            ReleaseClimbing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag == "Water") return;
        if (tag == "Protein") return;
        if (tag == "Level") return;
        if (tag == "FallingZone")
        {
            BeginVoidFall();
            return;
        }

        if (tag == "Desk") PlaceCameraLookingAtSreen();
    }

    private void BeginVoidFall()
    {
        if (CurrentState != State.Jumping)
        {
            ChangeState(State.Falling);
        }
    }

    public void StartTraining(TrainingData data, IPlayerTrainingHost host)
    {
        CurrentTrainingData = data;
        currentTrainingHost = host;
        transform.position = CurrentTrainingData.trainingPos.position;
        transform.rotation = CurrentTrainingData.trainingPos.rotation;
        SetTrainingType(data.playerTrainingType);
        ChangeState(State.Training);
        GameManager.Instance.TrainingStarted(data.playerTrainingType);
        Cursor.visible = true;
    }

    public void StopTraining()
    {
        Cursor.visible = false;
        transform.position = CurrentTrainingData.exitPos.position;
        transform.rotation = CurrentTrainingData.exitPos.rotation;
        GameManager.Instance.TrainingStopped();
        currentTrainingHost?.ReleaseTrainingSpot();
        currentTrainingHost = null;
        CurrentTrainingData = null;
        SetTrainingType(PlayerTrainingType.None);
        ChangeState(State.Walking);
    }

    private async void PlaceCameraLookingAtSreen()
    {
        Vector3 targetPosition = new Vector3(0, 1.04f, 0.6f);
        await Awaitable.WaitForSecondsAsync(3);
        // cameraPlace.localPosition = targetPosition;
    }
}
