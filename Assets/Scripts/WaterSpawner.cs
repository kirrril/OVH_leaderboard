using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> legsSpawnZones;
    [SerializeField] private List<BoxCollider> chestSpawnZones;
    [SerializeField] private List<BoxCollider> backSpawnZones;
    [SerializeField] private GameObject waterBottle;
    private Coroutine currentSpawnCoroutine;

    private void OnEnable()
    {
        GameManager.Instance.CurrentLevelChanged += SpawnWater;
    }

    private void OnDisable()
    {
        GameManager.Instance.CurrentLevelChanged -= SpawnWater;
    }

    public void SpawnWater()
    {
        if (currentSpawnCoroutine != null)
        {
            StopCoroutine(currentSpawnCoroutine);
            currentSpawnCoroutine = null;
        }
        waterBottle.SetActive(false);

        BoxCollider spawnZone = SetSpawnZone(GameManager.Instance.CurrentLevel);

        if (!spawnZone)
        {
            waterBottle.SetActive(false);
            return;
        }

        Vector3 spawnPoint = SetSpawnPoint(spawnZone);
        
        currentSpawnCoroutine = StartCoroutine(SpawnWaterCoroutine(spawnPoint));
    }

    private BoxCollider SetSpawnZone(CurrentLevelZone level)
    {
        switch (level)
        {
            case CurrentLevelZone.Legs:

                return legsSpawnZones[Random.Range(0, legsSpawnZones.Count)];
            case CurrentLevelZone.Chest:
                return chestSpawnZones[Random.Range(0, chestSpawnZones.Count)];
            case CurrentLevelZone.Back:
                return backSpawnZones[Random.Range(0, backSpawnZones.Count)];
            default:
                return null;
        }
    }

    private Vector3 SetSpawnPoint(BoxCollider zone)
    {
        Bounds bounds = zone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y + 0.3f;

        return new Vector3(x, y, z);
    }

    private IEnumerator SpawnWaterCoroutine(Vector3 point)
    {
        yield return new WaitForSeconds(3f);
        waterBottle.transform.position = point;
        waterBottle.SetActive(true);
        currentSpawnCoroutine = null;
    }
}