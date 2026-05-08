using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.Framework;

public class FightController : MonoBehaviour
{
    [SerializeField] private Transform manTransform;
    [SerializeField] private ManController manController;
    [SerializeField] private Animator manAnimator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject fightLight;
    private GameObject player;
    private PlayerController playerController;
    private Animator playerAnimator;
    [SerializeField] private Blackboard blackboard;

    void OnEnable()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponentInChildren<PlayerController>();
        playerAnimator = playerController.transform.GetComponentInChildren<Animator>();
        playerController.currentState = PlayerController.State.Fighting;
        playerController.isBeingAttacked = true;
        StartCoroutine(DoFighting());
    }

    void OnDisable()
    {
        playerController.isBeingAttacked = false;
        blackboard.SetVariableValue("hasInteracted", true);
        agent.enabled = true;
        agent.isStopped = false;
    }

    private IEnumerator DoFighting()
    {
        manAnimator.SetBool("isAttacking", true);
        Debug.Log("Ha!");
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            if (IsFacingEnemy() && playerController.playerAttack)
            {
                playerController.Push();
                manAnimator.SetBool("isAttacking", false);
                manAnimator.SetBool("isSubmissed", true);
                yield return new WaitForSeconds(2);
                GameManager.Instance.ModifyScore(10);
                playerController.currentState = PlayerController.State.Walking;
                yield return new WaitForSeconds(2);
                fightLight.SetActive(false);
                manAnimator.SetBool("isSubmissed", false);
                agent.enabled = true;
                agent.isStopped = false;
                gameObject.SetActive(false);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        playerController.SufferSubmission();
        yield return new WaitForSeconds(2);
        GameManager.Instance.ModifyScore(-10);
        GameManager.Instance.LoseHealth();
        playerAnimator.SetBool("isSubmissed", false);
        playerController.currentState = PlayerController.State.Dying;
        manAnimator.SetBool("isAttacking", false);
        fightLight.SetActive(false);
        agent.enabled = true;
        agent.isStopped = false;
        gameObject.SetActive(false);
    }

    bool IsFacingEnemy()
    {
        Vector3 directionToEnemy = (manTransform.position - playerController.transform.position).normalized;
        float angle = Vector3.Angle(playerController.transform.forward, directionToEnemy);
        return angle <= 20f;
    }
}
