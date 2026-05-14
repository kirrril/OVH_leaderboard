using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class Barbell : MonoBehaviour
{
    [SerializeField] private GameObject occupiedWall;
    [SerializeField] private TrainingSpot trainingSpot;
    [SerializeField] private Animator selfAnimator;
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
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, true);
        if (occupiedWall) occupiedWall.SetActive(true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, false);
        manController.StopTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(false);
        isAvailable = true;
    }
}