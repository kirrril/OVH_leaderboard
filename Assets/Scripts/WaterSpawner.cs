using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> legsSpawnZones;
    [SerializeField] private List<BoxCollider> chestSpawnZones;
    [SerializeField] private List<BoxCollider> backSpawnZones;
    [SerializeField] private GameObject waterBottle;
    private float respawnWaterLevel = 0.4f;

    private void OnEnable()
    {
        SubscribeToCurrentLevelChanged();
    }

    private void Start()
    {
        SubscribeToCurrentLevelChanged();
    }

    private void Update()
    {
        SpawnWater();
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= RemoveSpawnedBottle;
    }

    private void SubscribeToCurrentLevelChanged()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= RemoveSpawnedBottle;
        GameManager.Instance.CurrentLevelChanged += RemoveSpawnedBottle;
    }

    private void RemoveSpawnedBottle()
    {
        if (GameManager.Instance.CurrentLevel != CurrentLevelZone.None) return;

        waterBottle.SetActive(false);
    }

    public void SpawnWater()
    {
        if (GameManager.Instance.CurrentLevel == CurrentLevelZone.None) return;
        if (GameManager.Instance.Water > respawnWaterLevel) return;
        if (waterBottle.activeSelf) return;

        BoxCollider spawnZone = SetSpawnZone(GameManager.Instance.CurrentLevel);

        if (!spawnZone) return;

        Vector3 spawnPoint = SetSpawnPoint(spawnZone);
        waterBottle.transform.position = spawnPoint;
        waterBottle.SetActive(true);
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
        float y = bounds.min.y;

        return new Vector3(x, y, z);
    }
}
