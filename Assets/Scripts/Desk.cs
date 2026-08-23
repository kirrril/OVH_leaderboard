using UnityEngine;

public class Desk : MonoBehaviour
{
    public Transform typingCameraTarget;
    public Transform typingCameraPlace;
    public Transform screenCameraTarget;
    public Transform screenCameraPlace;
    public Transform gamingPos;
    public Transform exitPos;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject authentificationScreen;

    void OnTriggerEnter(Collider other)
    {
        if (other != playerController.mainCollider) return;
        playerController.StartGaming(this);
    }

    void OnTriggerExit(Collider other)
    {

    }

    public void ShowAuthentificationScreen()
    {
        authentificationScreen.SetActive(true);
    }
}