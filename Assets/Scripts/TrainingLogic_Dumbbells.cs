using System.Collections;
using UnityEngine;

public class TrainingLogic_Dumbbells : MonoBehaviour
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private Coroutine currentReleaseCoroutine;
    private PlayerController playerController;
    private bool isAvailable = true;
    private bool blockedByPlayer;

    [SerializeField] private string[] selfAnimationBools;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        if (selfAnimator != null)
        {
            foreach (string boolName in selfAnimationBools)
            {
                selfAnimator.SetBool(boolName, false);
            }
        }
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
        isAvailable = false;
        blockedByPlayer = true;
        return;
    }

    private void HandleManEnter(GameObject user)
    {
        if (!isAvailable) return;

        IAgent agent = user.GetComponent<IAgent>();
        if (agent == null) return;

        StartCoroutine(TrainMan(agent));
    }

    private void HandleGirlEnter(GameObject user)
    {
        if (!isAvailable) return;

        GirlController girl = user.GetComponent<GirlController>();
        if (girl == null) return;

        StartCoroutine(TrainGirl(girl));
    }

    private IEnumerator TrainMan(IAgent agent)
    {
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        agent.StartTraining(trainingData);
        selfAnimator.SetBool(selfAnimationBools[2], true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining();
        selfAnimator.SetBool(selfAnimationBools[2], false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        RequestSpotRelease();
    }

    private int GetSpotAnimationIndex()
    {
        return Random.Range(0, 2);
    }

    private GirlTrainingType GetGirlTrainingType(int index)
    {
        if (index == 0)
        {
            return GirlTrainingType.DumbbellsStand1;
        }
        else
        {
            return GirlTrainingType.DumbbellsStand2;
        }
    }

    private IEnumerator TrainGirl(GirlController girl)
    {
        int index = GetSpotAnimationIndex();
        GirlTrainingType trainingType = GetGirlTrainingType(index);

        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        
        girl.StartTraining(trainingData, trainingType);
        selfAnimator.SetBool(selfAnimationBools[index], true);

        if (occupiedObstacle) occupiedObstacle.SetActive(true);

        yield return new WaitForSeconds(trainingData.trainingDuration);

        girl.StopTraining();
        selfAnimator.SetBool(selfAnimationBools[index], false);

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