using System.Collections;
using UnityEngine;

public enum ExcludedUserKind { Player, Man, Girl, PlayerMan, PlayerGirl, AllAllowed }

public class TrainingLogic : MonoBehaviour, IPlayerTrainingHost
{
    [SerializeField] private ExcludedUserKind excludedUserKind;
    [SerializeField] private Animator selfAnimator;
    [SerializeField] private GameObject accessObstacle;
    [SerializeField] private GameObject occupiedObstacle;
    [SerializeField] private TrainingData trainingData;
    private PlayerController playerController;
    private bool isAvailable = true;
    private bool blockedByPlayer;

    void OnEnable()
    {
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject user;
        user = other.gameObject;

        switch (excludedUserKind)
        {
            case ExcludedUserKind.Player:
                ExcludePlayer(user);
                break;
            case ExcludedUserKind.Man:
                ExcludeMan(user);
                break;
            case ExcludedUserKind.Girl:
                ExcludeGirl(user);
                break;
            case ExcludedUserKind.PlayerMan:
                ExcludePlayerAndMan(user);
                break;
            case ExcludedUserKind.PlayerGirl:
                ExcludePlayerAndGirl(user);
                break;
            case ExcludedUserKind.AllAllowed:
                AllowAllUsers(user);
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!blockedByPlayer) return;

        blockedByPlayer = false;
        isAvailable = true;
    }

    private void ExcludeGirl(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (user.CompareTag("Girl"))
            {
                // agent.CancelTraining();
                return;
            }

            if (user.CompareTag("Man"))
            {
                if (!isAvailable)
                {
                    // agent.CancelTraining();
                    return;
                }
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (user.CompareTag("Player"))
        {
            TrainPlayer(user);
        }
    }

    private void ExcludeMan(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (user.CompareTag("Man"))
            {
                // agent.CancelTraining();
                return;
            }

            if (user.CompareTag("Girl"))
            {
                if (!isAvailable)
                {
                    // agent.CancelTraining();
                    return;
                }
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (user.CompareTag("Player"))
        {
            TrainPlayer(user.gameObject);
        }
    }

    private void ExcludePlayer(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (!isAvailable)
            {
                // agent.CancelTraining();
                return;
            }
            StartCoroutine(TrainAgent(agent));
            return;
        }

        if (user.CompareTag("Player"))
        {
            isAvailable = false;
            blockedByPlayer = true;
            return;
        }
    }

    private void ExcludePlayerAndGirl(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (user.CompareTag("Girl"))
            {
                // agent.CancelTraining();
                return;
            }

            if (user.CompareTag("Man"))
            {
                if (!isAvailable)
                {
                    // agent.CancelTraining();
                    return;
                }
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (user.CompareTag("Player"))
        {
            isAvailable = false;
            blockedByPlayer = true;
            return;
        }
    }

    private void ExcludePlayerAndMan(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (user.CompareTag("Man"))
            {
                // agent.CancelTraining();
                return;
            }

            if (user.CompareTag("Girl"))
            {
                if (!isAvailable)
                {
                    // agent.CancelTraining();
                    return;
                }
                StartCoroutine(TrainAgent(agent));
                return;
            }
        }

        if (user.CompareTag("Player"))
        {
            isAvailable = false;
            blockedByPlayer = true;
            return;
        }
    }

    private void AllowAllUsers(GameObject user)
    {
        if (user.CompareTag("Girl") || user.CompareTag("Man"))
        {
            IAgent agent;
            agent = user.GetComponent<IAgent>();

            if (!isAvailable)
            {
                // agent.CancelTraining();
                return;
            }
            StartCoroutine(TrainAgent(agent));
            return;
        }

        if (user.CompareTag("Player"))
        {
            TrainPlayer(user.gameObject);
        }
    }

    private IEnumerator TrainAgent(IAgent agent)
    {
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        agent.StartTraining(trainingData);
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
        yield return new WaitForSeconds(trainingData.trainingDuration);
        agent.StopTraining(trainingData);
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        if (accessObstacle) accessObstacle.SetActive(true);
        isAvailable = true;
    }

    private void TrainPlayer(GameObject player)
    {
        if (!isAvailable) return;
        isAvailable = false;
        if (accessObstacle) accessObstacle.SetActive(false);
        playerController = player.GetComponent<PlayerController>();
        playerController.StartTraining(trainingData, this);
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, true);
        if (occupiedObstacle) occupiedObstacle.SetActive(true);
    }

    public void ReleaseTrainingSpot()
    {
        if (selfAnimator) selfAnimator.SetBool(trainingData.selfAnimatorBool, false);
        if (occupiedObstacle) occupiedObstacle.SetActive(false);
        isAvailable = true;
        if (accessObstacle) accessObstacle.SetActive(true);
    }
}