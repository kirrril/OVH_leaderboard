using System.Collections;
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
    private bool playerJump;
    string trainingAnimationBool;

    bool cameraFreezed = false;
    Vector3 frozenCameraLocalPlace = new Vector3(0f, 0f, 0f);

    public enum State { Walking, Training, Fighting, Falling, DyingOfThirst, BeingSubmissed, Jumping, PushingTheDoor, ClimbingThePole, Dying, MakingDoubleSelfie };
    private string[] exclusiveAnimatorBools =
    {
        "isFalling",
        "isSubmissed",
        "isGaming",
        "isJogging",
        "isCycling",
        "isBoxJumping",
        "isPullingRower",
        "isMakingDips",
        "isPushingBarbell",
        "isTrainingChest_1",
        "isTrainingChest_2",
        "isPullingBackMachine1",
        "isPullingBackMachine2",
        "isExtensingBack",
        "isPullingBackBarbell1",
        "isMakingAustralianPullUps",
        "isMakingPullUps",
        "isMakingPullUps2"
    };

    public State currentState = State.Walking;


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

    public void ChangeState(State nextState)
    {
        if (currentState == nextState) return;

        currentState = nextState;
        ApplyStateAnimation(nextState);
    }

    private void ApplyStateAnimation(State state)
    {
        switch (state)
        {
            case State.Walking:
                ApplyExclusiveAnimatorBool();
                break;

            case State.Training:
                ApplyExclusiveAnimatorBool(trainingAnimationBool);
                break;

            case State.Falling:
                ApplyExclusiveAnimatorBool("isFalling");
                break;

            case State.BeingSubmissed:
                ApplyExclusiveAnimatorBool("isSubmissed");
                break;
        }
    }

    private void ApplyExclusiveAnimatorBool(string newBool = null)
    {
        foreach (string param in exclusiveAnimatorBools)
        {
            animator.SetBool(param, false);
        }

        if (!string.IsNullOrEmpty(newBool))
        {
            animator.SetBool(newBool, true);
        }
    }

    private void HandleWalking()
    {
        MovePlayer();
        RotatePlayer();
        MoveCameraTarget();
        CheckIfMoving();
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

    }

    private void HandlePushingTheDoor()
    {

    }

    private void HandleClimbingThePole()
    {

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

    private void CheckIfMoving()
    {
        if (Mathf.Abs(playerMovement.x) > 0.1f || Mathf.Abs(playerMovement.y) > 0.1f)
        {
            animator.SetFloat("MovementSpeed", 2.1f);
        }
        else
        {
            animator.SetFloat("MovementSpeed", 0.2f);
        }
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
        playerJump = ctx.ReadValueAsButton();
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
        trainingAnimationBool = trainingData.userAnimatorBool;
        ChangeState(State.Training);
        GameManager.Instance.TrainingStarted(data.progressType);
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
