using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform cameraTarget;
    [SerializeField]
    private Transform cameraPlace;
    private Vector2 playerMovement;
    private Vector2 mouseDelta;
    private Vector3 reinitCameraPlace = new Vector3(0f, 1.9f, -1f);
    private Vector3 reinitCameraTarget = new Vector3(0f, 1.7f, 0f);

    private bool isWalking = true;
    private bool isTraining;
    private bool playerAttack;
    private bool playerInteract;
    private bool playerJump;
    public int score = 80;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (isWalking)
        {
            MovePlayer();
            RotatePlayer();
            MoveCameraTarget();
            CheckIfMoving();
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    void MovePlayer()
    {
        if (!isWalking) return;
        Vector2 movementInput = playerMovement.normalized;
        Vector3 targetVelocity = transform.forward * movementInput.y * 1.5f + transform.right * movementInput.x * 1.5f;
        rb.linearVelocity = targetVelocity;
    }

    void RotatePlayer()
    {
        if (!isWalking) return;

        float yawDelta = mouseDelta.normalized.x * 3f;
        rb.angularVelocity = new Vector3(0, yawDelta, 0);
    }

    void MoveCameraTarget()
    {
        if (!isWalking) return;

        float pitchDelta = mouseDelta.normalized.y * 1.2f;
        pitchDelta = Mathf.Clamp(pitchDelta, -1f, 2f);
        float pitch = cameraTarget.localPosition.y + pitchDelta * 2 * Time.fixedDeltaTime;

        cameraTarget.localPosition = new Vector3(0, pitch, 0);
    }

    void CheckIfMoving()
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

    void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        Transform spot = other.transform;
        Transform trainingPos = spot.Find("TrainingPos");
        Transform exitPos = spot.Find("ExitPos"); ///////////////////////////
        GameObject wall = spot.Find("Wall")?.gameObject;

        string animBool = "";
        string scriptName = tag;
        Vector3 cameraPlacePosition;
        Vector3 cameraTargetPosition;

        switch (tag)
        {
            case "Desk": animBool = "isGaming"; cameraPlacePosition = new Vector3(-1f, 1.5f, 0.5f); cameraTargetPosition = new Vector3(0, 1.04f, 1f); break;
            case "Treadmill": animBool = "isJogging"; cameraPlacePosition = new Vector3(0.8f, 1.8f, -1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Bike": animBool = "isCycling"; cameraPlacePosition = new Vector3(-0.8f, 1.8f, 1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "JumpBox": animBool = "isBoxJumping"; cameraPlacePosition = new Vector3(0f, 2f, 2f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Rower": animBool = "isPullingRower"; cameraPlacePosition = new Vector3(-1f, 1.5f, 0.5f); cameraTargetPosition = new Vector3(0, 1.04f, 1f); break;
            case "Dips": animBool = "isMakingDips"; cameraPlacePosition = new Vector3(0.8f, 1.8f, -1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "Barbell": animBool = "isPushingBarbell"; cameraPlacePosition = new Vector3(0f, 2f, 0f); cameraTargetPosition = new Vector3(0, 0.5f, -0.7f); break;
            case "ChestMachine1": animBool = "isTrainingChest_1"; cameraPlacePosition = new Vector3(0f, 1.8f, 1.6f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            case "ChestMachine2": animBool = "isTrainingChest_2"; cameraPlacePosition = new Vector3(0f, 1.8f, 2f); cameraTargetPosition = new Vector3(0, 1.3f, 1f); break;
            default: return;
        }

        if (tag == "Desk")
        {
            PlaceCameraLookingAtSreen();
            return;
        }
        
        Train(spot, trainingPos, scriptName, animBool, cameraTargetPosition, cameraPlacePosition, wall);
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.tag;

        if (other.CompareTag("Rower"))
        {
            animator.SetBool("isPullingRower", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("JumpBox"))
        {
            animator.SetBool("isBoxJumping", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Bike"))
        {
            animator.SetBool("isCycling", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Treadmill"))
        {
            animator.SetBool("isJogging", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Dips"))
        {
            animator.SetBool("isMakingDips", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Barbell"))
        {
            animator.SetBool("isPushingBarbell", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("ChestMachine1"))
        {
            animator.SetBool("isTrainingChest_1", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("ChestMachine2"))
        {
            animator.SetBool("isTrainingChest_2", false);
            isTraining = false;
            isWalking = true;
            rb.isKinematic = false;
            Transform exit = other.transform.Find("ExitPos");
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            Vector3 target = new Vector3(0, 1.5f, 0.6f);
            Vector3 place = new Vector3(-1f, 2.1f, -1.2f);
            SetCamera(target, place);
        }
    }

    void Train(Transform spot, Transform trainingPos, string scriptName, string animationBool, Vector3 cameraTargetPosition, Vector3 cameraPlacePosition, GameObject wall)
    {
        isWalking = false;
        isTraining = true;
        rb.isKinematic = true;
        transform.position = trainingPos.position;
        transform.rotation = trainingPos.rotation;
        animator.SetBool(animationBool, true);
        SetCamera(cameraTargetPosition, cameraPlacePosition);
        if (wall != null) wall.SetActive(true);
        var spotController = spot.GetComponent(scriptName);
        if (spotController == null) return;
        var isAvailableField = spotController.GetType().GetField("isAvailable");
        isAvailableField.SetValue(spotController, false);
    }

    void StopTraining(Transform spot, Transform exitPos, string scriptName, string animationBool, GameObject wall)
    {
        animator.SetBool(animationBool, false);
        isTraining = false;
        isWalking = true;
        rb.isKinematic = false;
        transform.position = exitPos.position;
        transform.rotation = exitPos.rotation;
        SetCamera(reinitCameraTarget, reinitCameraPlace);
        if (wall != null) wall.SetActive(false);
        var spotController = spot.GetComponent(scriptName);
        if (spotController == null) return;
        var isAvailableField = spotController.GetType().GetField("isAvailable");
        isAvailableField.SetValue(spotController, true);
    }

    void SetCamera(Vector3 target, Vector3 place)
    {
        cameraTarget.localPosition = target;
        cameraPlace.localPosition = place;
    }

    async void PlaceCameraLookingAtSreen()
    {
        Vector3 targetPosition = new Vector3(0, 1.04f, 0.6f);

        await Awaitable.WaitForSecondsAsync(3);

        cameraPlace.localPosition = targetPosition;
    }
}
