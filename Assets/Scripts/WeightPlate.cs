using System.Collections;
using UnityEngine;

public class WeightPlate : MonoBehaviour
{
    private bool hasLanded;
    public Coroutine currentDespawnCoroutine;


    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        PlayerController playerController = collision.collider.GetComponentInParent<PlayerController>();

        if (playerController != null)
        {
            GameManager.Instance.RequestDeath(GameManager.DeathReason.BarbellWeight);
        }

        if (collision.collider.name == "PlatformBackFloor")
        {
            hasLanded = true;
            currentDespawnCoroutine = StartCoroutine(DespawnCoroutine());
        }
    }

    private IEnumerator DespawnCoroutine()
    {
        float randomDelay = Random.Range(3f, 10f);
        yield return new WaitForSeconds(randomDelay);
        currentDespawnCoroutine = null;
        hasLanded = false;
        gameObject.SetActive(false);
    }

}
