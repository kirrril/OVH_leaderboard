using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FightController : MonoBehaviour
{
    // [SerializeField] private Transform manTransform;
    // [SerializeField] private ManController manController;
    // [SerializeField] private Animator manAnimator;
    // [SerializeField] private NavMeshAgent agent;
    // [SerializeField] private GameObject fightLight;
    // private GameObject player;
    // private PlayerController playerController;
    // private Animator playerAnimator;

    // void OnEnable()
    // {
    //     player = GameObject.Find("PlayerPrefab");
    //     playerController = player.GetComponent<PlayerController>();
    //     playerAnimator = player.GetComponentInChildren<Animator>();
    //     playerController.currentState = PlayerController.State.Fighting;
    //     StartCoroutine(DoFighting());
    // }

    // void OnDisable()
    // {
    //     playerController.isBeingAttacked = false;
    //     manController.hasInteracted = false;
    //     agent.enabled = true;
    //     agent.isStopped = false;
    //     manController.currentState = ManController.State.MovingToTarget;
    // }

    // private IEnumerator DoFighting()
    // {
    //     manAnimator.SetBool("isAttacking", true);
    //     Debug.Log("Ha!");
    //     float elapsedTime = 0f;
    //     while (elapsedTime < 1f)
    //     {
    //         if (IsFacingEnemy() && playerController.playerAttack)
    //         {
    //             playerController.Push();
    //             manAnimator.SetBool("isAttacking", false);
    //             manAnimator.SetBool("isSubmissed", true);
    //             yield return new WaitForSeconds(2);
    //             playerController.score += 10;
    //             playerController.currentState = PlayerController.State.Walking;
    //             // yield return new WaitForSeconds(2);
    //             // playerController.isBeingAttacked = false;
    //             // manController.hasInteracted = false;
    //             fightLight.SetActive(false);
    //             // yield return new WaitForSeconds(2);
    //             manAnimator.SetBool("isSubmissed", false);
    //             // agent.enabled = true;
    //             // agent.isStopped = false;
    //             // manController.currentState = ManController.State.MovingToTarget;
    //             gameObject.SetActive(false);
    //             yield break;
    //         }
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    //     yield return new WaitForSeconds(2);
    //     playerController.SufferSubmission();
    //     yield return new WaitForSeconds(2);
    //     playerController.score -= 10;
    //     playerAnimator.SetBool("isSubmissed", false);
    //     playerController.currentState = PlayerController.State.Dying;
    //     manAnimator.SetBool("isAttacking", false);
    //     // playerController.isBeingAttacked = false;
    //     // manController.hasInteracted = false;
    //     // agent.enabled = true;
    //     // agent.isStopped = false;
    //     // manController.currentState = ManController.State.MovingToTarget;
    //     gameObject.SetActive(false);
    // }

    // bool IsFacingEnemy()
    // {
    //     Vector3 directionToEnemy = (manTransform.position - player.transform.position).normalized;
    //     float angle = Vector3.Angle(player.transform.forward, directionToEnemy);
    //     return angle <= 20f;
    // }
}
