using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    private float forwardSpeed = 2f, forwardSpeedCoeff = 1.7f, sideSpeed = 2f, rotationSpeed = 5f, rotationCoeff = 10f;
    private Vector2 playerMovement;
    private Vector2 playerRotation;
    private bool playerAttack;
    private bool playerInteract;
    private bool playerJump;
    // private bool isGaming;
    // public bool isMoving;

    void FixedUpdate()
    {
        if (!PlayerStates.isGaming)
        {
            MovePlayer();
            RotatePlayer();
            // LiftCamera();
        }

        if (PlayerStates.isGaming)
        {
            rb.angularVelocity = Vector3.zero;
        }

        CheckIfMoving();
    }

    void MovePlayer()
    {
        if (PlayerStates.isGaming) return;
        Vector2 movementInput = playerMovement.normalized;
        Vector3 targetVelocity = transform.forward * movementInput.y * forwardSpeed + transform.right * movementInput.x * sideSpeed;
        rb.linearVelocity = targetVelocity;
    }


    void RotatePlayer()
    {
        if (PlayerStates.isGaming) return;

        float rotationY = playerRotation.normalized.x * 6f;
        rb.angularVelocity = new Vector3(0, rotationY, 0);
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
}
