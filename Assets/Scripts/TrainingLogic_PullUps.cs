using System.Collections;
using UnityEngine;

public class TrainingLogic_PullUps : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData australianTrainingData;
    [SerializeField] private TrainingData assistedTrainingData;
    [SerializeField] private TrainingData regularTrainingData;
    private Coroutine currentReleaseCoroutine;
    private PlayerController playerController;
    private bool isAvailable = true;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (selfAnimator) selfAnimator.SetBool(assistedTrainingData.selfAnimatorBool, false);
    }

    void OnDisable()
    {
        currentReleaseCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject user;
        user = other.gameObject;

        AllowAllUsers(user);
    }

    private void AllowAllUsers(GameObject user)
    {
        if (user.CompareTag("Girl"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (!isAvailable)
            {
                return;
            }
            StartCoroutine(TrainAgent(agent, australianTrainingData));
            return;
        }

        if (user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (!isAvailable)
            {
                return;
            }
            StartCoroutine(TrainAgent(agent, regularTrainingData));
            return;
        }

        if (user.CompareTag("Player"))
        {
            TrainPlayer(user.gameObject);
        }
    }

    private IEnumerator TrainAgent(IAgent agent, TrainingData trainingData)
    {
        isAvailable = false;
        agent.StartTraining(trainingData);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining();
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        RequestSpotRelease();
    }

    private void TrainPlayer(GameObject player)
    {
        if (!isAvailable) return;
        isAvailable = false;
        playerController = player.GetComponent<PlayerController>();
        SelectPullUpsTraining();
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
    }

    private void SelectPullUpsTraining()
    {
        if (GameManager.Instance.PullUpsTraining < 0.33f)
        {
            playerController.StartTraining(australianTrainingData, this);
            if (selfAnimator) selfAnimator.SetBool(assistedTrainingData.selfAnimatorBool, false);
            return;
        }

        if (GameManager.Instance.PullUpsTraining > 0.66f)
        {
            playerController.StartTraining(regularTrainingData, this);
            if (selfAnimator) selfAnimator.SetBool(assistedTrainingData.selfAnimatorBool, false);
            return;
        }

        playerController.StartTraining(assistedTrainingData, this);
        if (selfAnimator) selfAnimator.SetBool(assistedTrainingData.selfAnimatorBool, true);
    }

    public void ReleaseTrainingSpot()
    {
        if (selfAnimator) selfAnimator.SetBool(assistedTrainingData.selfAnimatorBool, false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
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
        currentReleaseCoroutine = null;
    }
}