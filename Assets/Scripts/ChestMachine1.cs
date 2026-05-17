using System.Collections;
using UnityEngine;

public class ChestMachine1 : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    [SerializeField] private Animator selfAnimator;
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
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
    }

    private IEnumerator TrainMan(GameObject agent)
    {
        manController = agent.GetComponent<ManController>();
        manController.StartTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        manController.StopTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }

    private IEnumerator TrainGirl(GameObject agent)
    {
        girlController = agent.GetComponent<GirlController>();
        girlController.StartTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        girlController.StopTraining(trainingData);
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }

    public void ReleaseTrainingSpot()
    {
        selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        isAvailable = true;
    }
}
