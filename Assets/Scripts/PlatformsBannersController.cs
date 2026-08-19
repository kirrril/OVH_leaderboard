using UnityEngine;
using TMPro;

public class PlatformsBannersController : MonoBehaviour
{
    [SerializeField] private RectTransform legsBanner;
    [SerializeField] private TMP_Text legsBannerLeg;
    [SerializeField] private TMP_Text legsBannerZone;
    [SerializeField] private TMP_Text chestBannerChest;
    [SerializeField] private TMP_Text chestBannerZone;
    [SerializeField] private TMP_Text backBannerBack;
    [SerializeField] private TMP_Text backBannerZone;
    private Color currentLevelColor = new Color(1f, 1f, 1f, 1f);
    private Color nearLevelColor = new Color(1f, 1f, 1f, 0.7f);
    private Color farLevelColor = new Color(1f, 1f, 1f, 0.5f);

    private void OnEnable()
    {
        SubscribeToCurrentLevelChanged();
    }

    private void Start()
    {
        SubscribeToCurrentLevelChanged();
        RefreshBanners();
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= RefreshBanners;
    }


    private void SubscribeToCurrentLevelChanged()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CurrentLevelChanged -= RefreshBanners;
        GameManager.Instance.CurrentLevelChanged += RefreshBanners;
    }

    private void RefreshBanners()
    {
        SetBannerColors();
        SetLegsBannerRotation();
    }

    private void SetBannerColors()
    {
        switch (GameManager.Instance.CurrentLevel)
        {
            case CurrentLevelZone.Legs:
                SetBannerColor(legsBannerLeg, legsBannerZone, currentLevelColor);
                SetBannerColor(chestBannerChest, chestBannerZone, nearLevelColor);
                SetBannerColor(backBannerBack, backBannerZone, farLevelColor);
                break;

            case CurrentLevelZone.Chest:
                SetBannerColor(legsBannerLeg, legsBannerZone, nearLevelColor);
                SetBannerColor(chestBannerChest, chestBannerZone, currentLevelColor);
                SetBannerColor(backBannerBack, backBannerZone, nearLevelColor);
                break;

            case CurrentLevelZone.Back:
                SetBannerColor(legsBannerLeg, legsBannerZone, farLevelColor);
                SetBannerColor(chestBannerChest, chestBannerZone, nearLevelColor);
                SetBannerColor(backBannerBack, backBannerZone, currentLevelColor);
                break;

            default:
                SetBannerColor(legsBannerLeg, legsBannerZone, currentLevelColor);
                SetBannerColor(chestBannerChest, chestBannerZone, currentLevelColor);
                SetBannerColor(backBannerBack, backBannerZone, currentLevelColor);
                break;
        }
    }

    private void SetLegsBannerRotation()
    {
        if (legsBanner == null) return;

        bool flipBanner =
            GameManager.Instance.CurrentLevel == CurrentLevelZone.Chest ||
            GameManager.Instance.CurrentLevel == CurrentLevelZone.Back;

        legsBanner.localRotation = Quaternion.Euler(0f, flipBanner ? 180f : 0f, 0f);
    }

    private void SetBannerColor(TMP_Text textA, TMP_Text textB, Color color)
    {
        textA.color = color;
        textB.color = color;
    }
}
