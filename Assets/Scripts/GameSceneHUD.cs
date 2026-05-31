using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameSceneHUD : MonoBehaviour
{
    [SerializeField] private GameObject legsTraining;
    [SerializeField] private GameObject chestTraining;
    [SerializeField] private GameObject backTraining;
    [SerializeField] private GameObject legsProgress;
    [SerializeField] private GameObject legsCompleted;
    [SerializeField] private GameObject chestProgress;
    [SerializeField] private GameObject chestCompleted;
    [SerializeField] private GameObject backProgress;
    [SerializeField] private GameObject backCompleted;
    [SerializeField] private Image water;
    [SerializeField] private Image legs;
    [SerializeField] private Image chest;
    [SerializeField] private Image back;
    [SerializeField] private Image treadmill;
    [SerializeField] private Image bike;
    [SerializeField] private Image jumpbox;
    [SerializeField] private Image chestMachine1;
    [SerializeField] private Image chestMachine2;
    [SerializeField] private Image barbell;
    [SerializeField] private Image dips;
    [SerializeField] private Image rower;
    [SerializeField] private Image backExtension;
    [SerializeField] private Image backBarbell;
    [SerializeField] private Image backMachine1;
    [SerializeField] private Image backMachine2;
    [SerializeField] private Image pullups;
    [SerializeField] private Image healthPointsImage;
    [SerializeField] private Image thirstyDeathScreen;
    [SerializeField] private Image fallingDownScreen;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Sprite[] healthPointsSprites;

    private bool isVoidFalling;
    private bool isDyingOfThirst;


    void Update()
    {
        UpdateWaterLevel();
        VoidFallingScreen();
        UpdateHealthPoints();
        UpdateTrainingProgress();
        ManageTrainingPanels();
    }

    private void UpdateHealthPoints()
    {
        switch (GameManager.Instance.health)
        {
            case 5:
                healthPointsImage.sprite = healthPointsSprites[0];
                break;
            case 4:
                healthPointsImage.sprite = healthPointsSprites[1];
                break;
            case 3:
                healthPointsImage.sprite = healthPointsSprites[2];
                break;
            case 2:
                healthPointsImage.sprite = healthPointsSprites[3];
                break;
            case 1:
                healthPointsImage.sprite = healthPointsSprites[4];
                break;
        }
    }

    private void UpdateTrainingProgress()
    {
        legs.fillAmount = GameManager.Instance.LegsTraining;
        chest.fillAmount = GameManager.Instance.ChestTraining;
        back.fillAmount = GameManager.Instance.BackTraining;
        treadmill.fillAmount = GameManager.Instance.TreadmillTraining;
        bike.fillAmount = GameManager.Instance.BikeTraining;
        jumpbox.fillAmount = GameManager.Instance.JumpboxTraining;
        barbell.fillAmount = GameManager.Instance.BarbellTraining;
        chestMachine1.fillAmount = GameManager.Instance.ChestMachine1Training;
        chestMachine2.fillAmount = GameManager.Instance.ChestMachine2Training;
        dips.fillAmount = GameManager.Instance.DipsTraining;
        rower.fillAmount = GameManager.Instance.RowerTraining;
        backExtension.fillAmount = GameManager.Instance.BackExtensionTraining;
        backBarbell.fillAmount = GameManager.Instance.BackBarbell1Training;
        backMachine1.fillAmount = GameManager.Instance.BackMachine1Training;
        backMachine2.fillAmount = GameManager.Instance.BackMachine2Training;
        pullups.fillAmount = GameManager.Instance.PullUpsTraining;
    }

    private void ManageTrainingPanels()
    {
        switch (GameManager.Instance.CurrentLevel)
        {
            case CurrentLevelZone.Legs:
                legsTraining.SetActive(true);
                chestTraining.SetActive(false);
                backTraining.SetActive(false);
                ManageProgressOrCompleted(legsProgress, legsCompleted, GameManager.Instance.LegsTraining);
                break;
            case CurrentLevelZone.Chest:
                legsTraining.SetActive(false);
                chestTraining.SetActive(true);
                backTraining.SetActive(false);
                ManageProgressOrCompleted(chestProgress, chestCompleted, GameManager.Instance.ChestTraining);
                break;
            case CurrentLevelZone.Back:
                legsTraining.SetActive(false);
                chestTraining.SetActive(false);
                backTraining.SetActive(true);
                ManageProgressOrCompleted(backProgress, backCompleted, GameManager.Instance.BackTraining);
                break;
            case CurrentLevelZone.None:
                legsTraining.SetActive(false);
                chestTraining.SetActive(false);
                backTraining.SetActive(false);
                break;
        }
    }

    private void ManageProgressOrCompleted(GameObject progressPanel, GameObject completedPanel, float progressValue)
    {
        progressPanel.SetActive(progressValue < 1f);
        completedPanel.SetActive(progressValue >= 1f);
    }

    private void VoidFallingScreen()
    {
        if (playerController.currentState == PlayerController.State.Falling && !isVoidFalling)
        {
            isVoidFalling = true;
            StartCoroutine(FallingDownScreenCoroutine());
        }
    }

    public IEnumerator FallingDownScreenCoroutine()
    {
        fallingDownScreen.gameObject.SetActive(true);

        float duration = 1f;
        float elapsedTime = 0f;

        Color color = fallingDownScreen.color;
        color.a = 0f;
        fallingDownScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Clamp01(elapsedTime / duration);
            fallingDownScreen.color = color;

            yield return null;
        }
        color.a = 1f;
        fallingDownScreen.color = color;
        yield return new WaitForSeconds(2f);
        color.a = 0f;
        fallingDownScreen.color = color;

        fallingDownScreen.gameObject.SetActive(false);
        isVoidFalling = false;
    }

    private void UpdateWaterLevel()
    {
        water.fillAmount = GameManager.Instance.water;

        if (GameManager.Instance.water <= 0 && !isDyingOfThirst)
        {
            isDyingOfThirst = true;
            StartCoroutine(ThirstyDeathScreenCoroutine());
        }
    }

    public IEnumerator ThirstyDeathScreenCoroutine()
    {
        thirstyDeathScreen.gameObject.SetActive(true);

        float duration = 3f;
        float elapsedTime = 0f;

        Color color = thirstyDeathScreen.color;
        color.a = 0f;
        thirstyDeathScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Clamp01(elapsedTime / duration);
            thirstyDeathScreen.color = color;

            yield return null;
        }
        color.a = 1f;
        thirstyDeathScreen.color = color;
        yield return new WaitForSeconds(1f);
        color.a = 0f;
        thirstyDeathScreen.color = color;

        thirstyDeathScreen.gameObject.SetActive(false);
        isDyingOfThirst = false;
    }
}
