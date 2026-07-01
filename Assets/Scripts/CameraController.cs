using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] private PlayerController playerController;
    private Transform activeCameraPlace;
    private Transform activeCameraTarget;

    void OnEnable()
    {
        playerController.WarpTransition += NotifyTargetWarp;
    }

    void OnDisable()
    {
        playerController.WarpTransition -= NotifyTargetWarp;
    }

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

    public void NotifyTargetWarp(Transform newCameraPlace, Vector3 warpPositionDelta)
    {
        Debug.Log("NotifyTargetWarp called");
        virtualCamera.OnTargetObjectWarped(newCameraPlace, warpPositionDelta);
    }
}
