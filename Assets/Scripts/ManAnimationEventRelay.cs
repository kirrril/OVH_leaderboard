using UnityEngine;

public class ManAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private ManController manController;

    public void OnBlockedAnimationFinished()
    {
        manController.OnBlockedAnimationFinished();
    }

    public void OnDefeatAnimationFinished()
    {
        manController.OnDefeatAnimationFinished();
    }
}