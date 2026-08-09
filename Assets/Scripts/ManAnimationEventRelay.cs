using UnityEngine;

public class ManAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private ManController manController;

    public void OnDefeat()
    {
        manController.OnDefeat();
    }

    public void OnThrowFinished()
    {
        manController.OnThrowFinished();
    }
}
