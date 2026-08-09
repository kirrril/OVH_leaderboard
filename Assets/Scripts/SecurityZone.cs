using UnityEngine;

public class SecurityZone : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private int playerOverlapCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other != playerController.mainCollider) return;

        playerOverlapCount++;

        if (playerOverlapCount == 1)
        {
            playerController.EnterSecurityZone(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != playerController.mainCollider) return;

        playerOverlapCount--;

        if (playerOverlapCount <= 0)
        {
            playerOverlapCount = 0;
            playerController.ExitSecurityZone(this);
        }
    }
}