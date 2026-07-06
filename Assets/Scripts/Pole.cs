using UnityEngine;

public class Pole : MonoBehaviour
{
    public Transform climbingPos;
    public Transform cameraPlace;
    [SerializeField] private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.EnterClimbZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.ExitClimbZone();
    }
}