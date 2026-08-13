using System.Collections;
using UnityEngine;

public class WeightPlate : MonoBehaviour
{
    private bool hasLanded;
    [SerializeField] private LayerMask groundMask;
    public Coroutine currentDespawnCoroutine;


    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        PlayerController playerController = collision.collider.GetComponentInParent<PlayerController>();

        if (playerController != null)
        {
            GameManager.Instance.RequestDeath(GameManager.DeathReason.BarbellWeight);
        }

        if (collision.collider.name == "PlatformBack")
        {
            hasLanded = true;
            currentDespawnCoroutine = StartCoroutine(DespawnCoroutine());
        }
    }

    private IEnumerator DespawnCoroutine()
    {
        float randomDelay = Random.Range(3f, 10f);
        yield return new WaitForSeconds(randomDelay);
        hasLanded = false;
        gameObject.SetActive(false);
        currentDespawnCoroutine = null;
    }

}
