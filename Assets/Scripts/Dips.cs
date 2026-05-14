using System.Collections;
using UnityEngine;

public class Dips : MonoBehaviour
{
    [SerializeField] private GameObject noEntryWall;
    [SerializeField] private GameObject occupiedWall;
    [SerializeField] private TrainingSpot trainingSpot;
    private PlayerController playerController;
    private ManController manController;
    private bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (!isAvailable) return;

        if (other.CompareTag("Girl"))
        {
            return;
        }

        isAvailable = false;
        noEntryWall.SetActive(false);

        if (other.CompareTag("Man"))
        {
            StartCoroutine(TrainMan(other.gameObject));
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
}
