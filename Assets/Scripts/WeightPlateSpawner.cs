using System.Collections;
using UnityEngine;

public class WeightPlateSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider[] spawnZones;
    [SerializeField] private WeightPlate orangeWeightPlate;
    [SerializeField] private WeightPlate redWeightPlate;
    [SerializeField] private WeightPlate yellowWeightPlate;
    private Coroutine currentSpawnCoroutine;

    void Update()
    {
        SpawnWeightPlate(orangeWeightPlate);
        SpawnWeightPlate(redWeightPlate);
        SpawnWeightPlate(yellowWeightPlate);
    }

    public void SpawnWeightPlate(WeightPlate weightPlate)
    {
        if (GameManager.Instance.CurrentLevel != CurrentLevelZone.Back) return;
        if (GameManager.Instance.BackTraining < 0.3f) return;
        if (currentSpawnCoroutine != null) return;
        if (weightPlate.currentDespawnCoroutine != null) return;
        if (weightPlate.gameObject.activeSelf) return;
        currentSpawnCoroutine = StartCoroutine(SpawnCoroutine(weightPlate));

    }

    private Vector3 SetSpawnPoint(BoxCollider zone)
    {
        Bounds bounds = zone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y;

        return new Vector3(x, y, z);
    }

    private IEnumerator SpawnCoroutine(WeightPlate weightPlate)
    {
        float randomDelay = Random.Range(0f, 5f);
        yield return new WaitForSeconds(randomDelay);
        Vector3 spawnPoint = SetSpawnPoint(spawnZones[Random.Range(0, spawnZones.Length)]);
        weightPlate.transform.position = spawnPoint;
        weightPlate.gameObject.SetActive(true);
        currentSpawnCoroutine = null;
    }
}