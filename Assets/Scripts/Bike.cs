using System.Collections;
using UnityEngine;

public class Bike : MonoBehaviour
{
    [SerializeField] private GameObject occupiedWall;
    [SerializeField] private TrainingSpot trainingSpot;
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
        playerController.Train(trainingSpot);
    }

    private IEnumerator TrainMan(GameObject agent)
    {
        manController = agent.GetComponent<ManController>();
        manController.StartTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        manController.StopTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(false);
        isAvailable = true;
    }

    private IEnumerator TrainGirl(GameObject agent)
    {
        girlController = agent.GetComponent<GirlController>();
        girlController.StartTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        girlController.StopTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(false);
        isAvailable = true;
    }
}
