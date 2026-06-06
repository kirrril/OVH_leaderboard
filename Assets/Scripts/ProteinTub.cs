using UnityEngine;

public class ProteinTub : MonoBehaviour
{
    [SerializeField] private ProteinSpawner proteinSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.ModifyScore(1);
        proteinSpawner.SpawnProtein();
        gameObject.SetActive(false);
    }
}