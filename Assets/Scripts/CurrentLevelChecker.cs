using UnityEngine;
public enum CurrentLevelZone { None, Legs, Chest, Back }

public class CurrentLevelChecker : MonoBehaviour
{
    [SerializeField] private CurrentLevelZone currentLevelZone;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (currentLevelZone)
        {
            case CurrentLevelZone.None:
                GameManager.Instance.SetCurrentLevel(CurrentLevelZone.None);
                break;
            case CurrentLevelZone.Legs:
                GameManager.Instance.SetCurrentLevel(CurrentLevelZone.Legs);
                break;
            case CurrentLevelZone.Chest:
                GameManager.Instance.SetCurrentLevel(CurrentLevelZone.Chest);
                break;
            case CurrentLevelZone.Back:
                GameManager.Instance.SetCurrentLevel(CurrentLevelZone.Back);
                break;
        }
    }
}
