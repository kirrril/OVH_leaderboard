using UnityEngine;

public class WeightPlateDangerZone : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private int playerOverlapCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other != playerController.mainCollider) return;

        playerOverlapCount++;

        if (playerOverlapCount == 1)
        {
            playerController.EnterWeightPlateDangerZone(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != playerController.mainCollider) return;

        playerOverlapCount--;

        if (playerOverlapCount <= 0)
        {
            playerOverlapCount = 0;
            playerController.ExitWeightPlateDangerZone(this);
        }
    }
}