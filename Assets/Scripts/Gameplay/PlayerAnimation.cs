using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Update()
    {
        if (PlayerStates.isMoving)
        {
            animator.SetFloat("MovementSpeed", 2.1f);
        }

        if (PlayerStates.isMoving == false)
        {
            animator.SetFloat("MovementSpeed", 0.2f);
        }
    }
}
