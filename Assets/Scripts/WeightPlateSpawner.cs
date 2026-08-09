using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightPlateSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider[] spawnZones;
    [SerializeField] private WeightPlate[] weightPlates;
    private bool spawnStarted;

    void Update()
    {
        SpawnWeightPlate();
    }

    public void SpawnWeightPlate()
    {
        if (spawnStarted) return;
        spawnStarted = true;
        if (GameManager.Instance.CurrentLevel != CurrentLevelZone.Back) return;
        if (GameManager.Instance.BackTraining < 0.3f) return;

        Vector3 spawnPoint = SetSpawnPoint(spawnZones[Random.Range(0, spawnZones.Length)]);
        GameObject weightPlate = PickWeightPlateToSpawn();
        weightPlate.transform.position = spawnPoint;
        weightPlate.SetActive(true);
    }

    private GameObject PickWeightPlateToSpawn()
    {
        foreach (WeightPlate plate in weightPlates)
        {
            if (plate.currentSpawnCoroutine == null)
            {
                return plate.gameObject;
            }
        }

        return null;
    }

    private Vector3 SetSpawnPoint(BoxCollider zone)
    {
        Bounds bounds = zone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.min.y;

        return new Vector3(x, y, z);
    }



}