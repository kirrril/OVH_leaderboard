using System.Collections;
using UnityEngine;

public class TrainingLogic_Dumbbells : MonoBehaviour
{
    [SerializeField] private ExcludedUserKind excludedUserKind;
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private Coroutine currentReleaseCoroutine;
    private PlayerController playerController;
    private bool isAvailable = true;
    private bool blockedByPlayer;
    [SerializeField] private string[] agentAnimationBools;
    [SerializeField] private string[] selfAnimationBools;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        if (selfAnimator & trainingData.selfAnimatorBool != "") selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
    }

    void OnDisable()
    {
        currentReleaseCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject user;
        user = other.gameObject;

        ExcludePlayer(user);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!blockedByPlayer) return;
        
        RequestSpotRelease();
    }

    private void ExcludePlayer(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();
            string tag = user.tag;

            if (!isAvailable)
            {
                // agent.CancelTraining();
                return;
            }

            if (tag == "Girl")
            {
                int training = Random.Range(1, 3);
                trainingData.agentAnimatorBool = agentAnimationBools[training];
                trainingData.selfAnimatorBool = selfAnimationBools[training];
            }

            if (tag == "Man")
            {
                trainingData.agentAnimatorBool = agentAnimationBools[0];
                trainingData.selfAnimatorBool = selfAnimationBools[0];
            }

            StartCoroutine(TrainAgent(agent));
            return;
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
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining(trainingData);
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
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