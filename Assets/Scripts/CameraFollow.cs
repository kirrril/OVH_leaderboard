using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform cameraPlace;
    [SerializeField]
    private Transform cameraTarget;
    private float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        Vector3 desiredPos = cameraPlace.position;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
    }

    void LateUpdate()
    {
        Vector3 direction = cameraTarget.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 2.5f * Time.deltaTime);
    }
}
