using UnityEngine;

public class FightHurtbox : MonoBehaviour
{
    public enum HurtboxType { HeadFront, HeadLeft, HeadRight, HeadBack, ArmLeft, ArmRight, Chest }
    public HurtboxType hurtboxType;
    private PlayerController playerController;
    private FightZone fightZone;
    private FightHitbox fightHitbox;

    void OnEnable()
    {
        playerController = GameObject.Find("PlayerPrefab").GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root) return;

        fightZone = playerController.CurrentFightZone;
        if (fightZone == null) return;
        fightHitbox = other.gameObject.GetComponent<FightHitbox>();
        if (fightHitbox == null) return;

        fightZone.HurtboxTouched(fightHitbox.attackerType, hurtboxType);
    }
}