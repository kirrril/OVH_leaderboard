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
        if (selfAnimator) selfAnimator.SetBool("isMoving", false);
    }

    void OnDisable()
    {
        currentReleaseCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject user = other.gameObject;

        if (user.CompareTag("Player"))
        {
            HandlePlayerEnter(user);
            return;
        }

        if (user.CompareTag("Man"))
        {
            HandleManEnter(user);
            return;
        }

        if (user.CompareTag("Girl"))
        {
            HandleGirlEnter(user);
            return;
        }
    }

    private void HandlePlayerEnter(GameObject user)
    {
        TrainPlayer(user);
    }

    private void HandleManEnter(GameObject user)
    {
        if (!isAvailable) return;

        IAgent agent = user.GetComponent<IAgent>();
        if (agent == null) return;

        StartCoroutine(TrainAgent(agent, regularTrainingData));
    }

    private void HandleGirlEnter(GameObject user)
    {
        if (!isAvailable) return;

        IAgent agent = user.GetComponent<IAgent>();
        if (agent == null) return;

        StartCoroutine(TrainAgent(agent, australianTrainingData));
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
            return;
        }

        if (GameManager.Instance.PullUpsTraining > 0.66f)
        {
            playerController.StartTraining(regularTrainingData, this);
            return;
        }

        playerController.StartTraining(assistedTrainingData, this);
        if (selfAnimator) selfAnimator.SetBool("isMoving", true);
    }

    public void ReleaseTrainingSpot()
    {
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