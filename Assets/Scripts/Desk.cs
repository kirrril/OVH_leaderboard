using UnityEngine;

public class Desk : MonoBehaviour
{
    [SerializeField] private TrainingSpot trainingSpot;
    private PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        playerController = other.GetComponent<PlayerController>();
        playerController.Train(trainingSpot);
    }
}
