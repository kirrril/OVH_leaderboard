using System.Collections;
using UnityEngine;


public class TrainingLogic : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private Coroutine currentReleaseCoroutine;
    private PlayerController playerController;
    private bool isAvailable = true;
    private bool blockedByPlayer;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
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

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!blockedByPlayer) return;

        RequestSpotRelease();
    }

    private void HandlePlayerEnter(GameObject user)
    {
        if (trainingData.playerTrainingType == PlayerTrainingType.None)
        {
            isAvailable = false;
            blockedByPlayer = true;
            return;
        }

        TrainPlayer(user);
    }

    private void HandleManEnter(GameObject user)
    {
        if (!isAvailable) return;

        if (trainingData.manTrainingType == ManTrainingType.None) return;

        IAgent agent = user.GetComponent<IAgent>();
        if (agent == null) return;

        StartCoroutine(TrainAgent(agent));
    }

    private void HandleGirlEnter(GameObject user)
    {
        if (!isAvailable) return;

        if (trainingData.girlTrainingType == GirlTrainingType.None) return;

        IAgent agent = user.GetComponent<IAgent>();
        if (agent == null) return;

        StartCoroutine(TrainAgent(agent));
    }

    private IEnumerator TrainAgent(IAgent agent)
    {
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        agent.StartTraining(trainingData);
        if (selfAnimator) selfAnimator.SetBool("isMoving", true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining();
        if (selfAnimator) selfAnimator.SetBool("isMoving", false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        RequestSpotRelease();
    }

    private void TrainPlayer(GameObject player)
    {
        if (!isAvailable) return;
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        playerController = player.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
        if (selfAnimator) selfAnimator.SetBool("isMoving", true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
    }

    public void ReleaseTrainingSpot()
    {
        if (selfAnimator) selfAnimator.SetBool("isMoving", false);
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