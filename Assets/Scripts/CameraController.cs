using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] private PlayerController playerController;
    private Transform activeCameraPlace;
    private Transform activeCameraTarget;

    void Update()
    {
        SetActiveCameraTargetAndPlace();
    }

    private void SetActiveCameraTargetAndPlace()
    {
        switch (playerController.currentState)
        {
            case PlayerController.State.Training:
                {
                    if (!playerController.CurrentTrainingData)
                    {
                        activeCameraPlace = playerController.playerCameraPlace;
                        activeCameraTarget = playerController.playerCameraTarget;
                        break;
                    }
                    activeCameraPlace = playerController.CurrentTrainingData.cameraPlace;
                    activeCameraTarget = playerController.CurrentTrainingData.cameraTarget;
                }
                break;

            case PlayerController.State.Walking:
            case PlayerController.State.PushingTheDoor:
                {
                    if (!playerController.CurrentDoor)
                    {
                        activeCameraPlace = playerController.playerCameraPlace;
                        activeCameraTarget = playerController.playerCameraTarget;
                        break;
                    }
                    activeCameraPlace = playerController.CurrentDoor.cameraPlace;
                    activeCameraTarget = playerController.CurrentDoor.cameraTarget;
                }
                break;
            default:
                activeCameraPlace = playerController.playerCameraPlace;
                activeCameraTarget = playerController.playerCameraTarget;
                break;
        }

        virtualCamera.Follow = activeCameraPlace;
        virtualCamera.LookAt = activeCameraTarget;
    }
}
