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
    int lastSpotIndex = -1;
    private float fleeDistance = 3f;
    // private float chaseDistance = 5f;
    private bool isBusy;

    void Update()
    {
        if (!agent.enabled) return;
        UpdateWalkingSpeed();
        // if (FleePlayer()) return;

        if (!isBusy) SetNewDestination();
    }

    void UpdateWalkingSpeed()
    {
        float speed = new Vector3(agent.velocity.x, 0, agent.velocity.z).magnitude;
        float animationSpeed = speed > 0.05f ? 1.9f : 0f;
        animator.SetFloat("MovementSpeed", animationSpeed);
    }

    // private bool FleePlayer()
    // {
    //     float distance = Vector3.Distance(transform.position, player.position);
    //     if (distance >= fleeDistance) return false;

    //     isBusy = true;
    //     Vector3 fleeDir = (transform.position - player.position).normalized;
    //     NavMeshHit hit;
    //     if (NavMesh.SamplePosition(transform.position + fleeDir * 5f, out hit, 5f, NavMesh.AllAreas))
    //     {
    //         agent.SetDestination(hit.position);
    //     }
    //     isBusy = false;
    //     return true;
    // }

    // private void ChasePlayer()
    // {
    //     isBusy = true;
    //     float distance = Vector3.Distance(transform.position, player.position);
    //     if (distance < chaseDistance)
    //     {
    //         agent.SetDestination(player.position);
    //     }
    //     else isBusy = false;

    // }

    private void SetNewDestination()
    {
        isBusy = true;
        int targetIndex;
        do
        {
            targetIndex = Random.Range(0, trainingSpots.Length);
        } while (targetIndex == lastSpotIndex);

        lastSpotIndex = targetIndex;
        agent.SetDestination(trainingSpots[targetIndex].position);
    }

    private async void Train(GameObject wall, Transform training, Transform exit, string animationBool, int trainingDuration)
    {
        transform.position = training.position;
        transform.rotation = training.rotation;
        wall.SetActive(true);
        animator.SetBool(animationBool, true);
        await Awaitable.WaitForSecondsAsync(trainingDuration);
        wall.SetActive(false);
        animator.SetBool(animationBool, false);
        transform.position = exit.position;
        transform.rotation = exit.rotation;
        agent.enabled = true;
        agent.isStopped = false;
        isBusy = false;
    }



    private void OnTriggerEnter(Collider other)
    {
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        GameObject trainingSpot = other.gameObject;
        Transform training = trainingSpot.transform.Find("TrainingPos");
        Transform exit = trainingSpot.transform.Find("ExitPos");
        GameObject wall = trainingSpot.transform.Find("Wall")?.gameObject;
        string tag = other.tag;

        string scriptName = "";
        string animationBool = "";
        int duration = 0;

        switch (tag)
        {
            case "Treadmill": scriptName = "Treadmill"; animationBool = "isJogging"; duration = 8; break;
            case "Bike": scriptName = "Bike"; animationBool = "isCycling"; duration = 10; break;
            case "JumpBox": scriptName = "JumpBox"; animationBool = "isBoxJumping"; duration = 7; break;
            default: return;
        }

        var controllerScript = trainingSpot.GetComponent(scriptName);
        if (controllerScript == null || !(bool)controllerScript.GetType().GetField("isAvailable").GetValue(controllerScript))
        {
            transform.position = exit.position;
            transform.rotation = exit.rotation;
            agent.enabled = true;
            agent.isStopped = false;
            isBusy = false;
            return;
        }

        controllerScript.GetType().GetField("isAvailable").SetValue(controllerScript, false);

        Train(wall, training, exit, animationBool, duration);
    }
}
