using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProteinSpawner : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> legsSpawnZones;
    [SerializeField] private List<BoxCollider> chestSpawnZones;
    [SerializeField] private List<BoxCollider> backSpawnZones;
    [SerializeField] private GameObject proteinTub;
    private Coroutine currentSpawnCoroutine;

    private void OnEnable()
    {
        SubscribeToCurrentLevelChanged();
    }

    private void Start()
    {
        SubscribeToCurrentLevelChanged();
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= SpawnProtein;
    }

    private void SubscribeToCurrentLevelChanged()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= SpawnProtein;
        GameManager.Instance.CurrentLevelChanged += SpawnProtein;
    }

    public void SpawnProtein()
    {
        if (currentSpawnCoroutine != null)
        {
            StopCoroutine(currentSpawnCoroutine);
            currentSpawnCoroutine = null;
        }
        proteinTub.SetActive(false);

        BoxCollider spawnZone = SetSpawnZone(GameManager.Instance.CurrentLevel);

        if (!spawnZone)
        {
            proteinTub.SetActive(false);
            return;
        }

        Vector3 spawnPoint = SetSpawnPoint(spawnZone);
        
        currentSpawnCoroutine = StartCoroutine(SpawnProteinCoroutine(spawnPoint));
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

    private IEnumerator SpawnProteinCoroutine(Vector3 point)
    {
        yield return new WaitForSeconds(3f);
        proteinTub.transform.position = point;
        proteinTub.SetActive(true);
        currentSpawnCoroutine = null;
    }
}
