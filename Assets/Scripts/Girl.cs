using UnityEngine;
using UnityEngine.AI;

public class Girl : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform[] trainingSpots;

    void Start()
    {
        SetNewDestination();
    }

    private void SetNewDestination()
    {
        int targetIndex = Random.Range(0, trainingSpots.Length);
        Transform target = trainingSpots[targetIndex];
        agent.SetDestination(target.position);
    }

    private async void Train(GameObject trainingSpot, string scriptName, string trainingName, int trainingDuration)
    {
        var controllerScript = trainingSpot.GetComponent(scriptName);
        if (controllerScript != null && !(bool)controllerScript.GetType().GetField("isAvailable").GetValue(controllerScript))
        {
            SetNewDestination();
            return;
        }

        Transform training = trainingSpot.transform.Find("TrainingPos");
        Transform exit = trainingSpot.transform.Find("ExitPos");
        transform.position = training.position;
        transform.rotation = training.rotation;
        animator.SetBool(trainingName, true);
        await Awaitable.WaitForSecondsAsync(trainingDuration);
        transform.position = exit.position;
        transform.rotation = exit.rotation;
        animator.SetBool(trainingName, false);
        SetNewDestination();
    }

    private void OnTriggerEnter(Collider other)
    {
        agent.ResetPath();
        GameObject trainingSpot = other.gameObject;
        string tag = other.tag;

        switch (tag)
        {
            case "Girl":
                break;
            case "Treadmill":
                Train(trainingSpot, "Treadmill", "isJogging", 5);
                break;

            case "Bike":
                Train(trainingSpot, "Treadmill", "isCycling", 5);
                break;

            case "JumpBox":
                Train(trainingSpot, "Treadmill", "isBoxJumping", 5);
                break;

        }
    }
}
