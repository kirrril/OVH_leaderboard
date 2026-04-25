using System.Collections;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTarget;
    public Transform cameraPlace;
    [SerializeField] private GameObject stopTrainingButton;
    public Transform entryPoint;
    private Vector2 playerMovement;
    private Vector2 mouseDelta;
    private Vector3 reinitCameraPlace = new Vector3(0f, 1.9f, -1f);
    private Vector3 reinitCameraTarget = new Vector3(0f, 1.7f, 0f);
    Transform trainingSpot;
    Transform trainingPos;
    Transform exitPos;
    GameObject wall;
    string scriptName;
    string animationBool = "";

    public bool isBeingAttacked;
    private Transform enemy;

    public bool playerAttack;
    private bool playerInteract;
    private bool playerJump;
    public int score = 0;
    public int health = 5;

    public enum State { Walking, Training, Fighting, BeingSubmissed, Jumping, PushingTheDoor, ClimbingThePole, Dying, MakingDoubleSelfie };
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
                HandleDying();
                break;
            case State.MakingDoubleSelfie:
                HandleMakingDoubleSelfie();
                break;
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
        stopTrainingButton.SetActive(false);
    }

    private void HandleTraining()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        stopTrainingButton.SetActive(true);
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

    private void HandleDying()
    {
        health -= 1;
        transform.position = entryPoint.position;
        transform.rotation = entryPoint.rotation;
        SetCamera(reinitCameraTarget, reinitCameraPlace);
        currentState = State.Walking;
    }

    private void HandleMakingDoubleSelfie()
    {

    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerMovement.normalized;
        Vector3 targetVelocity = transform.forward * movementInput.y * 1.5f + transform.right * movementInput.x * 1.5f;
        rb.linearVelocity = targetVelocity;
    }

    private void RotatePlayer()
    {
        float yawDelta = mouseDelta.normalized.x * 3f;
        rb.angularVelocity = new Vector3(0, yawDelta, 0);
    }

    private void MoveCameraTarget()
    {
        float pitchDelta = mouseDelta.normalized.y * 1.2f;
        pitchDelta = Mathf.Clamp(pitchDelta, -1f, 2f);
        float pitch = cameraTarget.localPosition.y + pitchDelta * 2 * Time.fixedDeltaTime;

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

    public void ModifyScore(int delta)
    {
        score = Mathf.Max(0, score + delta);
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag == "Water") return;
        if (tag == "Protein") return;
        if (tag == "Level") return;

        trainingSpot = other.transform;
        trainingPos = trainingSpot.Find("TrainingPos");
        exitPos = trainingSpot.Find("ExitPos");
        wall = trainingSpot.Find("Wall")?.gameObject;
        scriptName = tag;
        Vector3 cameraPlacePosition;
        Vector3 cameraTargetPosition;

        switch (tag)
        {
            case "Desk": animationBool = "isGaming"; cameraPlacePosition = new Vector3(-1f, 1.5f, 0.5f); cameraTargetPosition = new Vector3(0, 1.04f, 1f); break;
            case "Treadmill": animationBool = "isJogging"; cameraPlacePosition = new Vector3(0.8f, 1.8f, -1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Bike": animationBool = "isCycling"; cameraPlacePosition = new Vector3(-0.8f, 1.8f, 1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "JumpBox": animationBool = "isBoxJumping"; cameraPlacePosition = new Vector3(0f, 2f, 2f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Rower": animationBool = "isPullingRower"; cameraPlacePosition = new Vector3(-1f, 1.5f, 0.5f); cameraTargetPosition = new Vector3(0, 1.04f, 1f); break;
            case "Dips": animationBool = "isMakingDips"; cameraPlacePosition = new Vector3(0.8f, 1.8f, -1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Barbell": animationBool = "isPushingBarbell"; cameraPlacePosition = new Vector3(0f, 2f, 0f); cameraTargetPosition = new Vector3(0, 0.5f, -0.7f); break;
            case "ChestMachine1": animationBool = "isTrainingChest_1"; cameraPlacePosition = new Vector3(0f, 1.8f, 1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "ChestMachine2": animationBool = "isTrainingChest_2"; cameraPlacePosition = new Vector3(0f, 1.8f, 2f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            default: return;
        }

        currentState = State.Training;

        Train(cameraTargetPosition, cameraPlacePosition);

        if (tag == "Desk") PlaceCameraLookingAtSreen();
    }

    private void Train(Vector3 cameraTargetPosition, Vector3 cameraPlacePosition)
    {
        rb.isKinematic = true;
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;
        animator.SetBool(animationBool, true);
        SetCamera(cameraTargetPosition, cameraPlacePosition);
        if (wall != null) wall.SetActive(true);
        var spotController = trainingSpot.GetComponent(scriptName);
        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return;
        isAvailableField.SetValue(spotController, false);
        stopTrainingButton.SetActive(true);
        Cursor.visible = true;
    }

    public void StopTraining()
    {
        stopTrainingButton.SetActive(false);
        Cursor.visible = false;
        animator.SetBool(animationBool, false);
        animationBool = "";
        rb.isKinematic = false;
        transform.position = exitPos.position;
        transform.rotation = exitPos.rotation;
        SetCamera(reinitCameraTarget, reinitCameraPlace);
        if (wall != null) wall.SetActive(false);
        var spotController = trainingSpot.GetComponent(scriptName);
        var isAvailableField = spotController.GetType().GetField("isAvailable");
        if (isAvailableField == null) return;
        isAvailableField.SetValue(spotController, true);
        currentState = State.Walking;
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
        currentState = State.BeingSubmissed;
        SetCamera(new Vector3(0, 0, 0), new Vector3(0, 2, 1));
        animator.SetBool("isSubmissed", true);
    }
}
