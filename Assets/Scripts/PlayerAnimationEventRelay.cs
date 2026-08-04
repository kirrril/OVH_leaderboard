using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void OnDefeat()
    {
        playerController.OnDefeat();
    }

    public void OnThrowFinished()
    {
        playerController.OnThrowFinished();
    }
}
