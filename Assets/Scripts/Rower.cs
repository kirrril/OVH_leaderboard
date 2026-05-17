using System.Collections;
using UnityEngine;

public class Rower : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private TrainingData trainingData;
    [SerializeField] private Animator selfAnimator;
    private PlayerController playerController;
    private GirlController girlController;
    private bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (!isAvailable) return;

        if (other.CompareTag("Man"))
        {
            return;
        }

        isAvailable = false;

        if (other.CompareTag("Girl"))
        {
            StartCoroutine(TrainGirl(other.gameObject));
            return;
        }

        accessObstacle.SetActive(false);
        playerController = other.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
    }

    public void ReleaseTrainingSpot()
    {
        occupiedObstacle.SetActive(false);
        isAvailable = true;
        accessObstacle.SetActive(true);
    }

    private IEnumerator TrainGirl(GameObject agent)
    {
        accessObstacle.SetActive(false);
        girlController = agent.GetComponent<GirlController>();
        girlController.StartTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        girlController.StopTraining(trainingData);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
        accessObstacle.SetActive(true);
    }
}
