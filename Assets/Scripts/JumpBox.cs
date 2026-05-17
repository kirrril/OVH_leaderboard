using System.Collections;
using UnityEngine;

public class JumpBox : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private PlayerController playerController;
    private ManController manController;
    private GirlController girlController;
    private bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (!isAvailable) return;

        isAvailable = false;

        if (other.CompareTag("Man"))
        {
            StartCoroutine(TrainMan(other.gameObject));
            return;
        }

        if (other.CompareTag("Girl"))
        {
            StartCoroutine(TrainGirl(other.gameObject));
            return;
        }

        playerController = other.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
    }

    private IEnumerator TrainMan(GameObject agent)
    {
        manController = agent.GetComponent<ManController>();
        manController.StartTraining(trainingData);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        manController.StopTraining(trainingData);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }

    private IEnumerator TrainGirl(GameObject agent)
    {
        girlController = agent.GetComponent<GirlController>();
        girlController.StartTraining(trainingData);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        girlController.StopTraining(trainingData);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }

    public void ReleaseTrainingSpot()
    {
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }
}
