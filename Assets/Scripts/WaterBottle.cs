using UnityEngine;

public class WaterBottle : MonoBehaviour
{
    [SerializeField] private WaterSpawner waterSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.water = 1f;
        waterSpawner.SpawnWater();
        gameObject.SetActive(false);
    }
}