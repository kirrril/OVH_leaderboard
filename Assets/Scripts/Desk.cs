using UnityEngine;

public class Desk : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private TrainingData trainingData;
    private PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        playerController = other.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
    }

    public void ReleaseTrainingSpot()
    {

    }
}
