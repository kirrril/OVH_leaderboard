using System.Collections;
using UnityEngine;

public class Dips : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingSpot trainingSpot;
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
                agent.ResetPath();
                return;
            }

            if (other.CompareTag("Man"))
            {
                if (!isAvailable)
                {
                    agent.ResetPath();
                    return;
                }
                isAvailable = false;
                accessObstacle.SetActive(false);
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
        agent.StartTraining(trainingSpot);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        agent.StopTraining(trainingSpot);
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
        playerController.Train(trainingSpot, this);
    }

    public void ReleaseTrainingSpot()
    {
        occupiedObstacle.SetActive(false);
        isAvailable = true;
        accessObstacle.SetActive(true);
    }
}
