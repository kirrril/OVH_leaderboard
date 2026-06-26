using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform pushingPos;
    [SerializeField] private PlayerController playerController;
    public Transform cameraPlace;
    public Transform cameraTarget;
    public Animator selfAnimator;
    private bool isInDoorZone = false;
    private bool doorIsOpen = false;

    void OnEnable()
    {
        playerController.OpenTheDoor += PlayOpenningAnimation;
    }

    void OnDisable()
    {
        playerController.OpenTheDoor -= PlayOpenningAnimation;
    }

    void Update()
    {
        CheckIfIsInDoorZone();
    }

    private void PlayOpenningAnimation()
    {
        selfAnimator.SetBool("isOpenning", true);
        doorIsOpen = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        isInDoorZone = true;
        playerController.EnterDoorZone(this);
    }

    private void CheckIfIsInDoorZone()
    {
        if (!isInDoorZone) return;

        Vector2 doorPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(playerController.transform.position.x, playerController.transform.position.z);

        if (Vector2.Distance(playerPos, doorPos) > 2f)
        {
            LeaveTheDoorZone();
        }
    }

    private void LeaveTheDoorZone()
    {
        playerController.ExitDoorZone();
        isInDoorZone = false;

        if (doorIsOpen)
        {
            selfAnimator.SetBool("isOpenning", false);
            doorIsOpen = false;
        }
    }
}