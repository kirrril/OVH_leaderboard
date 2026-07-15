using System.Collections;
using UnityEngine;

public class TrainingLogic_mobile : MonoBehaviour
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private GameObject mobile;
    [SerializeField] private TrainingData trainingData;
    private Coroutine currentReleaseCoroutine;
    private PlayerController playerController;
    private bool isAvailable = true;
    private bool blockedByPlayer;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        // if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        if (mobile) mobile.SetActive(false);
    }

    void OnDisable()
    {
        currentReleaseCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject user;
        user = other.gameObject;

        ExcludePlayerAndMan(user);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!blockedByPlayer) return;

        RequestSpotRelease();
    }

    private void ExcludePlayerAndMan(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (user.CompareTag("Man"))
            {
                return;
            }

            if (user.CompareTag("Girl"))
            {
                if (!isAvailable)
                {
                    return;
                }
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (user.CompareTag("Player"))
        {
            isAvailable = false;
            blockedByPlayer = true;
            return;
        }
    }

    private IEnumerator TrainAgent(IAgent agent)
    {
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        agent.StartTraining(trainingData);
        mobile.SetActive(true);
        // if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining();
        // if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        mobile.SetActive(false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        RequestSpotRelease();
    }

    private void RequestSpotRelease()
    {
        if (currentReleaseCoroutine != null) return;
        currentReleaseCoroutine = StartCoroutine(ReleaseSpotCoroutine());
    }

    private IEnumerator ReleaseSpotCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        isAvailable = true;
        if (blockedByPlayer) blockedByPlayer = false;
        currentReleaseCoroutine = null;
    }
}