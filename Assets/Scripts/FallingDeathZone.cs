using UnityEngine;

public class FallingDeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.RequestDeath(GameManager.DeathReason.VoidFall);
    }
}