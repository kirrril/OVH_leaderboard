using UnityEngine;

public class Desk : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private TrainingSpot trainingSpot;
    private PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        playerController = other.GetComponent<PlayerController>();
        playerController.Train(trainingSpot, this);
    }

    public void ReleaseTrainingSpot()
    {

    }
}
