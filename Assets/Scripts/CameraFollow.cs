using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform cameraPlace;
    [SerializeField]
    private Transform cameraTarget;
    private float smoothSpeed = 0.05f;

    void FixedUpdate()
    {
        Vector3 desiredPos = cameraPlace.position;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
    }

    void LateUpdate()
    {
        // transform.LookAt(cameraTarget);
        Vector3 direction = cameraTarget.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
    }
}
