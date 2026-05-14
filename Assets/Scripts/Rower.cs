using System.Collections;
using UnityEngine;

public class Rower : MonoBehaviour
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject noEntryWall;
    [SerializeField] private GameObject occupiedWall;
    [SerializeField] private TrainingSpot trainingSpot;
    private PlayerController playerController;
    private GirlController girlController;
    private bool isAvailable = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Man"))
        {
            return;
        }

        if (!isAvailable) return;

        isAvailable = false;
        noEntryWall.SetActive(false);

        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            playerController.Train(trainingSpot);
            return;
        }

        StartCoroutine(TrainAgent(other.gameObject));
    }

    private IEnumerator TrainAgent(GameObject agent)
    {
        girlController = agent.GetComponent<GirlController>();
        girlController.StartTraining(trainingSpot);
        if (occupiedWall) occupiedWall.SetActive(true);
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        girlController.StopTraining(trainingSpot);
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, false);
        if (occupiedWall) occupiedWall.SetActive(false);
        isAvailable = true;
        noEntryWall.SetActive(false);
    }
}
