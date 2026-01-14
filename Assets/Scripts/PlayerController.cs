using System.Threading.Tasks;
using Unity.Cinemachine;
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
    private Vector2 playerRotation;
    private bool isWalking = true;
    private bool isTraining;
    private bool playerAttack;
    private bool playerInteract;
    private bool playerJump;

    void FixedUpdate()
    {
        if (isWalking)
        {
            MovePlayer();
            RotatePlayer();
            LiftCameraTarget();
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

        float rotationY = playerRotation.normalized.x * 1.7f;
        rb.angularVelocity = new Vector3(0, rotationY, 0);
    }

    void LiftCameraTarget()
    {
        if (isTraining) return;

        float delta = playerRotation.y * 0.25f * Time.fixedDeltaTime;
        float newY = cameraTarget.localPosition.y + delta;
        newY = Mathf.Clamp(newY, 1.5f, 2.5f);
        cameraTarget.localPosition = new Vector3(0, newY, 0);
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
        playerRotation = ctx.ReadValue<Vector2>();
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
        if (other.CompareTag("Desk"))
        {
            isWalking = false;
            rb.isKinematic = true;
            Transform gaming = other.transform.Find("GamingPos");
            transform.position = gaming.position;
            transform.rotation = gaming.rotation;
            animator.SetBool("isGaming", true);
            Vector3 target = new Vector3(0, 1.04f, 1f);
            Vector3 place = new Vector3(-1f, 1.5f, 0.5f);
            SetCamera(target, place);
            PlaceCameraLookingAtSreen();
        }

        if (other.CompareTag("Rower"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isPullingRower", true);
            Vector3 target = new Vector3(0, 1.04f, 1f);
            Vector3 place = new Vector3(-1f, 1.5f, 0.5f);
            SetCamera(target, place);
        }

        if (other.CompareTag("JumpBox"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isBoxJumping", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0f, 2f, 2f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Bike"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isCycling", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(-0.8f, 1.8f, 1.6f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Treadmill"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isJogging", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0.8f, 1.8f, -1.6f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Dips"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isMakingDips", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0.8f, 1.8f, -1.6f);
            SetCamera(target, place);
        }

        if (other.CompareTag("Barbell"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isPushingBarbell", true);
            Vector3 target = new Vector3(0, 0.5f, -0.7f);
            Vector3 place = new Vector3(0f, 2f, 0f);
            SetCamera(target, place);
        }

        if (other.CompareTag("ChestMachine1"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isTrainingChest_1", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0f, 1.8f, 1.6f);
            SetCamera(target, place);
        }

        if (other.CompareTag("ChestMachine2"))
        {
            isWalking = false;
            isTraining = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            animator.SetBool("isTrainingChest_2", true);
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0f, 1.8f, 2f);
            SetCamera(target, place);
        }
    }

    void OnTriggerExit(Collider other)
    {
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
