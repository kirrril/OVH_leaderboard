using UnityEngine;

public class WaterBottle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.RefillWater();
        gameObject.SetActive(false);
    }
}