using System.Collections;
using UnityEngine;

public class ChestMachine2 : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private PlayerController playerController;
    private bool isAvailable = true;

    void OnEnable()
    {
        occupiedObstacle.SetActive(false);
        accessObstacle.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Girl") || other.CompareTag("Man"))
        {
            IAgent agent;
            agent = other.gameObject.GetComponent<IAgent>();

            if (other.CompareTag("Girl"))
            {
                agent.CancelTraining();
                return;
            }

            if (other.CompareTag("Man"))
            {
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (other.CompareTag("Player"))
        {
            TrainPlayer(other.gameObject);
        }
    }

    private IEnumerator TrainAgent(IAgent agent)
    {
        if (!isAvailable) yield break;
        isAvailable = false;
        accessObstacle.SetActive(false);
        agent.StartTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        accessObstacle.SetActive(true);
        isAvailable = true;
    }

    private void TrainPlayer(GameObject player)
    {
        if (!isAvailable) return;

        isAvailable = false;
        accessObstacle.SetActive(false);
        playerController = player.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
    }

    public void ReleaseTrainingSpot()
    {
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
        accessObstacle.SetActive(true);
    }
}
