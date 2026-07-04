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

    public bool isBeingAttacked;
    private Transform enemy;

    public bool playerAttack;
    private bool playerInteract;

    private JumpZone currentJumpZone;

    private float landedTimer;
    public float JumpChargeCoeff { get; private set; }

    public Door CurrentDoor { get; private set; }
    private float doorTimer;

    private Pole currentPole;
    float fullClimbingHeight = 20f;
    float climbingHeightLimit;

    public enum State { Walking, Gaming, Training, Fighting, Falling, BeingSubmissed, Jumping, PushingTheDoor, ClimbingThePole, MakingDoubleSelfie };
    public enum WalkingPhase { None, Idle, Walking };
    public enum JumpPhase { None, Charging, Squatting, Released, Airborne, Landed };
    public enum DoorPhase { None, Pushing, Releasing }
    public enum ClimbPhase { None, ClimbingUp, SlidingDown }

    public State currentState = State.Walking;
    public WalkingPhase walkingPhase;
    public JumpPhase jumpPhase;
    public DoorPhase doorPhase;
    public ClimbPhase climbPhase;
    public TrainingType CurrentTrainingType { get; private set; }

    private bool isGrounded;
    [SerializeField] private LayerMask groundMask;

    public event Action OpenTheDoor;


    void FixedUpdate()
    {
        switch (currentState)
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
            case State.BeingSubmissed:
                HandleBeingSubmissed();
                break;
            case State.MakingDoubleSelfie:
                HandleMakingDoubleSelfie();
                break;
        }
    }

    public void ChangeState(State nextState)
    {
        if (currentState == nextState) return;
        ResetSubStatesLeavingState(currentState);
        currentState = nextState;
        SetKinematicProperty(currentState);
        if (currentState == State.Walking) ReinitCameraPlace();
    }

    private void ReinitCameraPlace()
    {
        // if (currentState != State.Walking) return;
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
            case State.BeingSubmissed:
                rb.isKinematic = false;
                break;
            case State.MakingDoubleSelfie:
                rb.isKinematic = false;
                break;
        }
    }

    private void ChangeWalkingPhase(WalkingPhase nextPhase)
    {
        if (walkingPhase == nextPhase) return;

        walkingPhase = nextPhase;
    }

    private void ChangeJumpPhase(JumpPhase nextPhase)
    {
        if (jumpPhase == nextPhase) return;

        jumpPhase = nextPhase;
    }

    private void ChangeDoorPhase(DoorPhase nextPhase)
    {
        if (doorPhase == nextPhase) return;

        doorPhase = nextPhase;
    }

    private void ChangeClimbPhase(ClimbPhase nextPhase)
    {
        if (climbPhase == nextPhase) return;

        climbPhase = nextPhase;
    }

    private void SetTrainingType(TrainingType type)
    {
        if (CurrentTrainingType == type) return;

        CurrentTrainingType = type;
    }

    private void ResetSubStatesLeavingState(State state)
    {
        switch (state)
        {
            case State.Walking:
                walkingPhase = WalkingPhase.None;
                break;

            case State.Jumping:
                jumpPhase = JumpPhase.None;
                break;

            case State.PushingTheDoor:
                doorPhase = DoorPhase.None;
                break;

            case State.ClimbingThePole:
                climbPhase = ClimbPhase.None;
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
            SetTrainingType(TrainingType.None);
        }

        CurrentDoor = null;
        currentPole = null;
        climbingHeightLimit = 0f;
        currentJumpZone = null;

        walkingPhase = WalkingPhase.None;
        jumpPhase = JumpPhase.None;
        doorPhase = DoorPhase.None;
        climbPhase = ClimbPhase.None;
    }
    private void HandleJumping()
    {
        switch (jumpPhase)
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

        if (jumpPhase != JumpPhase.Airborne) return;
        if (CheckIfIsGrounded())
        {
            landedTimer = 0.5f;
            ChangeJumpPhase(JumpPhase.Landed);
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
        if (doorPhase == DoorPhase.Releasing) WaitAndWalk();

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
        if (currentPole == null)
        {
            ChangeState(State.Walking);
            return;
        }

        MoveCameraTarget();

        float climbingMaxHeight = currentPole.transform.position.y + climbingHeightLimit;

        switch (climbPhase)
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

    private void HandleBeingSubmissed()
    {
        rb.angularVelocity = Vector3.zero;
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

    public void EnterJumpZone(JumpZone jumpZone)
    {
        currentJumpZone = jumpZone;
        if (jumpZone.jumpingPos) transform.position = currentJumpZone.jumpingPos.position;
    }

    public void ExitJumpZone(JumpZone jumpZone)
    {
        if (currentJumpZone != jumpZone) return;
        currentJumpZone = null;
    }

    private void ChargeJump()
    {
        if (currentJumpZone == null) return;
        JumpChargeCoeff += Time.fixedDeltaTime / 6;
        JumpChargeCoeff = Mathf.Clamp(JumpChargeCoeff, 0f, 1f);
    }

    private void StartChargingJump()
    {
        JumpChargeCoeff = 0f;
        ChangeState(State.Jumping);
        if (currentJumpZone.jumpType == JumpZone.JumpType.Charged)
        {
            ChangeJumpPhase(JumpPhase.Charging);
            transform.position = currentJumpZone.jumpingPos.position;
        }
        else
        {
            ChangeJumpPhase(JumpPhase.Squatting);
        }
    }

    private void ReleaseJump()
    {
        Debug.Log(JumpChargeCoeff);

        if (jumpPhase != JumpPhase.Charging && jumpPhase != JumpPhase.Squatting) return;

        ChangeJumpPhase(JumpPhase.Released);

        switch (currentJumpZone.jumpType)
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
        currentPole = pole;
        climbingHeightLimit = fullClimbingHeight * GameManager.Instance.BackTraining;
    }

    public void ExitClimbZone()
    {
        currentPole = null;
        climbingHeightLimit = 0f;
    }

    private void StartClimbing()
    {
        transform.position = currentPole.climbingPos.position;
        ChangeState(State.ClimbingThePole);
        ChangeClimbPhase(ClimbPhase.ClimbingUp);
    }

    private void ReleaseClimbing()
    {
        ChangeClimbPhase(ClimbPhase.SlidingDown);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        playerMovement = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        playerAttack = ctx.ReadValueAsButton();
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        playerInteract = ctx.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (currentJumpZone == null) return;

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
        if (currentPole == null) return;

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
        if (currentState != State.Jumping)
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
        SetTrainingType(data.trainingType);
        ChangeState(State.Training);
        GameManager.Instance.TrainingStarted(data.trainingType);
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
        SetTrainingType(TrainingType.None);
        ChangeState(State.Walking);
    }

    private async void PlaceCameraLookingAtSreen()
    {
        Vector3 targetPosition = new Vector3(0, 1.04f, 0.6f);
        await Awaitable.WaitForSecondsAsync(3);
        // cameraPlace.localPosition = targetPosition;
    }

    public void SufferSubmission()
    {
        ChangeState(State.BeingSubmissed);
    }
}
