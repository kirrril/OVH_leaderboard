using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform pushingPos;
    [SerializeField] private PlayerController playerController;
    public Animator selfAnimator;

    void OnEnable()
    {
        playerController.OpenTheDoor += PlayOpenningAnimation;
    }

    void OnDisable()
    {
        playerController.OpenTheDoor -= PlayOpenningAnimation;
    }

    private void PlayOpenningAnimation()
    {
        selfAnimator.SetBool("isOpenning", true);
    }

    private void PlayClosingAnimation()
    {
        selfAnimator.SetBool("isOpenning", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.EnterDoorZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        playerController.ExitDoorZone();
        PlayClosingAnimation();
    }
}