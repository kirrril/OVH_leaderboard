using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform cameraTarget;
    [SerializeField]
    private Transform cameraPlace;
    private Vector2 playerMovement;
    private Vector2 playerRotation;
    private bool playerAttack;
    private bool playerInteract;
    private bool playerJump;

    void FixedUpdate()
    {
        if (PlayerStates.isWalking)
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
        if (PlayerStates.isGaming) return;
        Vector2 movementInput = playerMovement.normalized;
        Vector3 targetVelocity = transform.forward * movementInput.y * 1.5f + transform.right * movementInput.x * 1.5f;
        rb.linearVelocity = targetVelocity;
    }

    void RotatePlayer()
    {
        if (PlayerStates.isGaming) return;

        float rotationY = playerRotation.normalized.x * 1.7f;
        rb.angularVelocity = new Vector3(0, rotationY, 0);
    }

    void LiftCameraTarget()
    {
        if (PlayerStates.isGaming) return;
        if (PlayerStates.isTraining) return;

        float delta = playerRotation.y * 0.25f * Time.fixedDeltaTime;
        float newY = cameraTarget.localPosition.y + delta;
        newY = Mathf.Clamp(newY, 1.5f, 2.5f);
        cameraTarget.localPosition = new Vector3(0, newY, 0);
    }

    void CheckIfMoving()
    {
        if (Mathf.Abs(playerMovement.x) > 0.1f || Mathf.Abs(playerMovement.y) > 0.1f)
        {
            PlayerStates.isMoving = true;
        }
        else
        {
            PlayerStates.isMoving = false;
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
            PlayerStates.isWalking = false;
            PlayerStates.isGaming = true;
            rb.isKinematic = true;
            Transform gaming = other.transform.Find("GamingPos");
            transform.position = gaming.position;
            transform.rotation = gaming.rotation;
            Vector3 target = new Vector3(0, 1.04f, 1f);
            Vector3 place = new Vector3(-1f, 1.5f, 0.5f);
            SetCamera(target, place);
            PlaceCameraLookingAtSreen();
        }

        if (other.CompareTag("Rower"))
        {
            PlayerStates.isWalking = false;
            PlayerStates.isGaming = false;
            PlayerStates.isRowing = true;
            PlayerStates.isBoxJumping = false;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            Vector3 target = new Vector3(0, 1.04f, 1f);
            Vector3 place = new Vector3(-1f, 1.5f, 0.5f);
            SetCamera(target, place);
        }

        if (other.CompareTag("JumpBox"))
        {
            PlayerStates.isWalking = false;
            PlayerStates.isGaming = false;
            PlayerStates.isRowing = false;
            PlayerStates.isBoxJumping = true;
            rb.isKinematic = true;
            Transform training = other.transform.Find("TrainingPos");
            transform.position = training.position;
            transform.rotation = training.rotation;
            Vector3 target = new Vector3(0, 1.3f, 1f);
            Vector3 place = new Vector3(0f, 2f, 2f);
            SetCamera(target, place);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rower"))
        {
            PlayerStates.isWalking = true;
            PlayerStates.isGaming = false;
            PlayerStates.isRowing = false;
            PlayerStates.isBoxJumping = false;
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
            PlayerStates.isWalking = true;
            PlayerStates.isGaming = false;
            PlayerStates.isRowing = false;
            PlayerStates.isBoxJumping = false;
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
