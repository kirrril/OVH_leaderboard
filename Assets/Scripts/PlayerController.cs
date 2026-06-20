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
    [SerializeField] private Transform cameraTarget;
    public Transform cameraPlace;
    [SerializeField] private GameObject stopTrainingControl;
    public Transform entryPoint;
    private Vector2 playerMovement;
    private Vector2 mouseDelta;
    private Vector3 reinitCameraPlace = new Vector3(0f, 1.9f, -1f);
    private Vector3 reinitCameraTarget = new Vector3(0f, 1.7f, 0f);

    private TrainingData trainingData;
    private IPlayerTrainingHost trainingHost;

    public bool isBeingAttacked;
    private Transform enemy;

    public bool playerAttack;
    private bool playerInteract;

    private JumpZone currentJumpZone;
    private float landedTimer;
    private float jumpingCoeff;

    private Door currentDoor;
    private float doorTimer;

    private Pole currentPole;

    bool cameraFreezed = false;
    Vector3 frozenCameraLocalPlace = new Vector3(0f, 0f, 0f);

    public enum State { Walking, Gaming, Training, Fighting, Falling, DyingOfThirst, BeingSubmissed, Jumping, PushingTheDoor, ClimbingThePole, Dying, MakingDoubleSelfie };
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
            case State.DyingOfThirst:
                HandleDyingOfThirst();
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
            case State.Dying:
                HandleComeBack();
                break;
            case State.MakingDoubleSelfie:
                HandleMakingDoubleSelfie();
                break;
        }
    }

    private void ChangeState(State nextState)
    {
        if (currentState == nextState) return;
        ResetSubStatesLeavingState(currentState);
        currentState = nextState;
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
        cameraFreezed = false;
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
        if (!CheckIfIsGrounded())
            ChangeJumpPhase(JumpPhase.Airborne);
    }

    private void HandleJumpAirborne()
    {
        if (jumpPhase != JumpPhase.Airborne) return;
        if (CheckIfIsGrounded())
        {
            landedTimer = 1f;
            ChangeJumpPhase(JumpPhase.Landed);
        }
    }

    private void HandleJumpLanded()
    {
        landedTimer -= Time.fixedDeltaTime;

        if (landedTimer > 0f) return;

        ChangeJumpPhase(JumpPhase.None);
        ChangeState(State.Walking);
    }

    private bool CheckIfIsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void HandlePushingTheDoor()
    {
        if (doorPhase == DoorPhase.Releasing) WaitAndWalk();
    }

    private void WaitAndWalk()
    {
        doorTimer -= Time.fixedDeltaTime;
        if (doorTimer > 0) return;
        ChangeState(State.Walking);
    }

    private void HandleClimbingThePole()
    {
        float climbingHeight = currentPole.climbingPos.position.y;

        switch (climbPhase)
        {
            case ClimbPhase.ClimbingUp:
                climbingHeight += Time.deltaTime * 2;
                break;
            case ClimbPhase.SlidingDown:
                climbingHeight -= Time.deltaTime * 2;
                break;
        }
        currentPole.climbingPos.position = new Vector3(currentPole.climbingPos.position.x, climbingHeight, currentPole.climbingPos.position.z);
        transform.position = currentPole.climbingPos.position;
    }

    private void HandleBeingSubmissed()
    {
        rb.angularVelocity = Vector3.zero;
    }

    private void HandleFalling()
    {
        if (!cameraFreezed)
        {
            frozenCameraLocalPlace = cameraPlace.localPosition;
            cameraFreezed = true;
        }
        frozenCameraLocalPlace += Vector3.forward * Time.deltaTime * 3;
        frozenCameraLocalPlace.z = Mathf.Min(frozenCameraLocalPlace.z, 0f);
        cameraPlace.localPosition = frozenCameraLocalPlace;
        animator.SetBool("isFalling", true);

        rb.angularVelocity = Vector3.zero;
    }

    public void HandleDyingOfThirst()
    {
        if (trainingData != null)
        {
            StopTraining();
        }
        HandleComeBack();
    }

    public void HandleComeBack()
    {
        transform.position = entryPoint.position;
        transform.rotation = entryPoint.rotation;
        SetCamera(reinitCameraTarget, reinitCameraPlace);
        ChangeState(State.Walking);
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
        float pitch = cameraTarget.localPosition.y + pitchDelta * 1.5f * Time.fixedDeltaTime;

        cameraTarget.localPosition = new Vector3(0, pitch, 0);
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
        jumpingCoeff += Time.fixedDeltaTime * 2;
    }

    private void StartChargingJump()
    {
        jumpingCoeff = 1f;
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
        if (jumpPhase != JumpPhase.Charging && jumpPhase != JumpPhase.Squatting) return;
        ChangeJumpPhase(JumpPhase.Released);
        rb.linearVelocity = transform.forward * 2 * jumpingCoeff + transform.up * 2 * jumpingCoeff;
    }

    public void EnterDoorZone(Door door)
    {
        currentDoor = door;
    }

    public void ExitDoorZone()
    {
        currentDoor = null;
        ChangeDoorPhase(DoorPhase.None);
        ChangeState(State.Walking);
    }

    private void StartPushing()
    {
        transform.position = currentDoor.pushingPos.position;
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
            doorTimer = 1f;
        }
    }

    public void EnterClimbZone(Pole pole)
    {
        currentPole = pole;
        Debug.Log(currentPole);
    }

    public void ExitClimbZone()
    {
        currentPole = null;
    }

    private void StartClimbing()
    {
        Debug.Log("Start training");
        rb.isKinematic = true;
        transform.position = currentPole.climbingPos.position;
        climbPhase = ClimbPhase.ClimbingUp;
        ChangeState(State.ClimbingThePole);
        ChangeClimbPhase(ClimbPhase.ClimbingUp);
    }

    private void ReleaseClimbing()
    {
        climbPhase = ClimbPhase.SlidingDown;
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
        if (currentDoor == null) return;

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
            ChangeState(State.Falling);
            return;
        }

        if (tag == "Desk") PlaceCameraLookingAtSreen();
    }

    public void StartTraining(TrainingData data, IPlayerTrainingHost host)
    {
        trainingData = data;
        trainingHost = host;
        rb.isKinematic = true;
        transform.position = trainingData.trainingPos.position;
        transform.rotation = trainingData.trainingPos.rotation;
        SetCamera(trainingData.cameraTargetLocalPosition, trainingData.cameraPlaceLocalPosition);
        SetTrainingType(data.trainingType);
        ChangeState(State.Training);
        GameManager.Instance.TrainingStarted(data.trainingType);
        Cursor.visible = true;
    }

    public void StopTraining()
    {
        Cursor.visible = false;
        rb.isKinematic = false;
        transform.position = trainingData.exitPos.position;
        transform.rotation = trainingData.exitPos.rotation;
        SetCamera(reinitCameraTarget, reinitCameraPlace);
        GameManager.Instance.TrainingStopped();
        trainingHost?.ReleaseTrainingSpot();
        trainingHost = null;
        trainingData = null;
        SetTrainingType(TrainingType.None);
        ChangeState(State.Walking);
    }

    private void SetCamera(Vector3 target, Vector3 place)
    {
        cameraTarget.localPosition = target;
        cameraPlace.localPosition = place;
    }

    private async void PlaceCameraLookingAtSreen()
    {
        Vector3 targetPosition = new Vector3(0, 1.04f, 0.6f);
        await Awaitable.WaitForSecondsAsync(3);
        cameraPlace.localPosition = targetPosition;
    }

    public void Push()
    {
        StartCoroutine(DoPush());
    }

    private IEnumerator DoPush()
    {
        animator.SetBool("isPushing", true);
        animator.SetFloat("PushingState", 0.5f);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("PushingState", 1.9f);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("PushingState", 0.1f);
        animator.SetBool("isPushing", false);
    }

    public void SufferSubmission()
    {
        ChangeState(State.BeingSubmissed);
        SetCamera(new Vector3(0, 0, 0), new Vector3(0, 2, 1));
    }
}
