using System;
using System.Collections;
using UnityEngine;

public class BarbellStand : MonoBehaviour
{
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingSpot trainingSpot;
    private bool isAvailable = true;
    private bool blockedByPlayer;

    void OnEnable()
    {
        occupiedObstacle.SetActive(false);
        accessObstacle.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        IAgent agent;

        if (other.CompareTag("Girl") || other.CompareTag("Man"))
        {
            agent = other.gameObject.GetComponent<IAgent>();

            if (other.CompareTag("Girl"))
            {
                agent.ResetPath();
                return;
            }

            if (other.CompareTag("Man"))
            {
                if (!isAvailable)
                {
                    agent.ResetPath();
                    return;
                }
                isAvailable = false;
                accessObstacle.SetActive(false);
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (other.CompareTag("Player"))
        {
            isAvailable = false; // player bloque le spot auquel il n'a pas accès pour ne pas se faire enfermé dans OccupiedObstacle
            blockedByPlayer = true; // flag pour savoir que c'est bien le player qui a bloqué le spot
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!blockedByPlayer) return; // si c'est le player qui a bloqué le spot

        blockedByPlayer = false;
        isAvailable = true; // c'est le player qui le débloque en sortant du trigger
    }

    private IEnumerator TrainAgent(IAgent agent)
    {
        agent.StartTraining(trainingSpot);
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, true);
        occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingSpot.trainingDuration);
        agent.StopTraining(trainingSpot);
        selfAnimator.SetBool(trainingSpot.selfAnimatorBool, false);
        occupiedObstacle.SetActive(false);
        accessObstacle.SetActive(true);
        isAvailable = true;
    }
}
