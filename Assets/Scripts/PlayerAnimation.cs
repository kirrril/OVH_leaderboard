using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Update()
    {
        float walkingSpeed = PlayerStates.isMoving ? 2.1f : 0.2f;
        animator.SetFloat("MovementSpeed", walkingSpeed);

        animator.SetBool("isGaming", PlayerStates.isGaming);
    }
}
