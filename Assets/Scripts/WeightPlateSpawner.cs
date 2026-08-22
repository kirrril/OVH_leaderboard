using System.Collections;
using UnityEngine;

public class WeightPlateSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BoxCollider spawnZone;
    [SerializeField] private WeightPlate weightPlate;
    [SerializeField] private MeshRenderer weightPlateMeshRenderer;
    [SerializeField] private Material[] weightPlateMeshMaterials;
    private Coroutine currentSpawnCoroutine;

    void Update()
    {
        SpawnWeightPlate();
    }

    public void SpawnWeightPlate()
    {
        
        if (playerController.CurrentWeightPlateDangerZone == null) return;
        if (GameManager.Instance.BackTraining < 0.3f) return;
        transform.position = playerController.transform.position + new Vector3(0, 6, 0);
        if (currentSpawnCoroutine != null) return;
        if (weightPlate.currentDespawnCoroutine != null) return;
        
        if (weightPlate.gameObject.activeSelf) return;        
        
        weightPlateMeshRenderer.material = weightPlateMeshMaterials[Random.Range(0, weightPlateMeshMaterials.Length)];
        currentSpawnCoroutine = StartCoroutine(SpawnCoroutine(weightPlate));

    }

    private Vector3 SetSpawnPoint()
    {
        Bounds bounds = spawnZone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y;

        return new Vector3(x, y, z);
    }

    private IEnumerator SpawnCoroutine(WeightPlate weightPlate)
    {
        float randomDelay = Random.Range(0f, 5f);
        yield return new WaitForSeconds(randomDelay);
        Vector3 spawnPoint = SetSpawnPoint();
        weightPlate.transform.position = spawnPoint;
        weightPlate.gameObject.SetActive(true);
        weightPlate.gameObject.transform.rotation = Quaternion.Euler(60, 0, 0);
        currentSpawnCoroutine = null;
    }
}