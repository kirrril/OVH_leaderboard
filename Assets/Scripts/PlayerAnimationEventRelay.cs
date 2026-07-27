using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void OnDefeatAnimationFinished()
    {
        playerController.OnDefeatAnimationFinished();
    }

    public void OnThrowFinished()
    {
        playerController.OnThrowFinished();
    }
}
