using System.Collections;
using UnityEngine;

public class WeightPlate : MonoBehaviour
{
    public Coroutine currentSpawnCoroutine;
    [SerializeField] private LayerMask groundMask;
    private bool hasLanded;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        PlayerController playerController = collision.collider.GetComponentInParent<PlayerController>();

        if (playerController != null)
        {
            GameManager.Instance.RequestDeath(GameManager.DeathReason.BarbellWeight);
        }

        if (((1 << collision.collider.gameObject.layer) & groundMask.value) != 0)
        {
            hasLanded = true;
            currentSpawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
