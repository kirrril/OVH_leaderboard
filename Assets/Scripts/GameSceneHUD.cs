using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameSceneHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text currentScoreValue;
    [SerializeField] private GameObject legsTraining;
    [SerializeField] private GameObject chestTraining;
    [SerializeField] private GameObject backTraining;
    [SerializeField] private GameObject legsProgress;
    [SerializeField] private GameObject legsCompleted;
    [SerializeField] private GameObject chestProgress;
    [SerializeField] private GameObject chestCompleted;
    [SerializeField] private GameObject backProgress;
    [SerializeField] private GameObject backCompleted;
    [SerializeField] private GameObject treadmillProgress;
    [SerializeField] private GameObject treadmillCompleted;
    [SerializeField] private GameObject bikeProgress;
    [SerializeField] private GameObject bikeCompleted;
    [SerializeField] private GameObject jumpboxProgress;
    [SerializeField] private GameObject jumpboxCompleted;
    [SerializeField] private GameObject dipsProgress;
    [SerializeField] private GameObject dipsCompleted;
    [SerializeField] private GameObject pecflyProgress;
    [SerializeField] private GameObject pecflyCompleted;
    [SerializeField] private GameObject benchpressProgress;
    [SerializeField] private GameObject benchpressCompleted;
    [SerializeField] private GameObject crossoverProgress;
    [SerializeField] private GameObject crossoverCompleted;
    [SerializeField] private GameObject cablerowProgress;
    [SerializeField] private GameObject cablerowCompleted;
    [SerializeField] private GameObject latpullProgress;
    [SerializeField] private GameObject latpullCompleted;
    [SerializeField] private GameObject rowerProgress;
    [SerializeField] private GameObject rowerCompleted;
    [SerializeField] private GameObject backextensionProgress;
    [SerializeField] private GameObject backextensionCompleted;
    [SerializeField] private GameObject pullupsProgress;
    [SerializeField] private GameObject pullupsCompleted;
    [SerializeField] private GameObject tbarProgress;
    [SerializeField] private GameObject tbarCompleted;
    [SerializeField] private Image water;
    [SerializeField] private Image waterPanel;
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
    [SerializeField] private Image healthPanel;

    [SerializeField] private Image[] legsProgressBarPanels;
    [SerializeField] private Image[] chestProgressBarPanels;
    [SerializeField] private Image[] backProgressBarPanels;

    private bool isVoidFalling;
    private bool isDyingOfThirst;
    private float waterAlertTimer;


    void Update()
    {
        UpdateCurrentScoreValue();
        UpdateWaterLevel();
        ActivateWaterLevelAlert();
        VoidFallingScreen();
        UpdateHealthPoints();
        ActivateHealthAlert();
        UpdateTrainingProgress();
        ManageTrainingPanels();
        HighlightProgressBar();
    }

    private void UpdateCurrentScoreValue()
    {
        currentScoreValue.text = GameManager.Instance.CurrentScore.ToString();
    }

    private void UpdateHealthPoints()
    {
        switch (GameManager.Instance.Health)
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

    private void ActivateHealthAlert()
    {
        if (GameManager.Instance.Health <= 1)
        {
            healthPanel.color = new Color(1f, 0.3f, 0.3f, 0.12f);
            return;
        }

        healthPanel.color = new Color(1f, 1f, 1f, 0.08f);
    }

    private void UpdateTrainingProgress()
    {
        legs.fillAmount = GameManager.Instance.LegsTraining;
        chest.fillAmount = GameManager.Instance.ChestTraining;
        back.fillAmount = GameManager.Instance.BackTraining;
        treadmill.fillAmount = GameManager.Instance.TreadmillTraining;
        bike.fillAmount = GameManager.Instance.BikeTraining;
        jumpbox.fillAmount = GameManager.Instance.JumpboxTraining;
        barbell.fillAmount = GameManager.Instance.BenchPressTraining;
        chestMachine1.fillAmount = GameManager.Instance.PecFlyTraining;
        chestMachine2.fillAmount = GameManager.Instance.CrossoverTraining;
        dips.fillAmount = GameManager.Instance.DipsTraining;
        rower.fillAmount = GameManager.Instance.RowerTraining;
        backExtension.fillAmount = GameManager.Instance.BackExtensionTraining;
        backBarbell.fillAmount = GameManager.Instance.TBarTraining;
        backMachine1.fillAmount = GameManager.Instance.LatPullTraining;
        backMachine2.fillAmount = GameManager.Instance.CableRowTraining;
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
                ManageProgressOrCompleted(treadmillProgress, treadmillCompleted, GameManager.Instance.TreadmillTraining);
                ManageProgressOrCompleted(bikeProgress, bikeCompleted, GameManager.Instance.BikeTraining);
                ManageProgressOrCompleted(jumpboxProgress, jumpboxCompleted, GameManager.Instance.JumpboxTraining);
                break;
            case CurrentLevelZone.Chest:
                legsTraining.SetActive(false);
                chestTraining.SetActive(true);
                backTraining.SetActive(false);
                ManageProgressOrCompleted(chestProgress, chestCompleted, GameManager.Instance.ChestTraining);
                ManageProgressOrCompleted(dipsProgress, dipsCompleted, GameManager.Instance.DipsTraining);
                ManageProgressOrCompleted(pecflyProgress, pecflyCompleted, GameManager.Instance.PecFlyTraining);
                ManageProgressOrCompleted(crossoverProgress, crossoverCompleted, GameManager.Instance.CrossoverTraining);
                ManageProgressOrCompleted(benchpressProgress, benchpressCompleted, GameManager.Instance.BenchPressTraining);
                break;
            case CurrentLevelZone.Back:
                legsTraining.SetActive(false);
                chestTraining.SetActive(false);
                backTraining.SetActive(true);
                ManageProgressOrCompleted(backProgress, backCompleted, GameManager.Instance.BackTraining);
                ManageProgressOrCompleted(latpullProgress, latpullCompleted, GameManager.Instance.LatPullTraining);
                ManageProgressOrCompleted(cablerowProgress, cablerowCompleted, GameManager.Instance.CableRowTraining);
                ManageProgressOrCompleted(rowerProgress, rowerCompleted, GameManager.Instance.RowerTraining);
                ManageProgressOrCompleted(backextensionProgress, backextensionCompleted, GameManager.Instance.BackExtensionTraining);
                ManageProgressOrCompleted(tbarProgress, tbarCompleted, GameManager.Instance.TBarTraining);
                ManageProgressOrCompleted(pullupsProgress, pullupsCompleted, GameManager.Instance.PullUpsTraining);
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

    private void HighlightProgressBar()
    {
        switch (GameManager.Instance.CurrentLevel)
        {
            case CurrentLevelZone.Legs:
                switch (GameManager.Instance.CurrentTrainingType)
                {
                    case TrainingProgressType.Treadmill:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.Bike:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.JumpBox:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.None:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        break;
                }
                break;
            case CurrentLevelZone.Chest:
                switch (GameManager.Instance.CurrentTrainingType)
                {
                    case TrainingProgressType.Barbell:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.ChestMachine1:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.ChestMachine2:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.Dips:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[3].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.None:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        break;
                }
                break;
            case CurrentLevelZone.Back:
                switch (GameManager.Instance.CurrentTrainingType)
                {
                    case TrainingProgressType.BackMachine1:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.BackMachine2:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.Rower:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.BackExtension:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[3].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.BackBarbell1:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[4].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.PullUps:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[5].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingProgressType.None:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        break;
                }
                break;
        }
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

    private void ActivateWaterLevelAlert()
    {
        if (GameManager.Instance.Water > 0.4f)
        {
            waterPanel.color = new Color(1f, 1f, 1f, 0.08f);
            waterAlertTimer = 0f;
            return;
        }

        if (playerController.currentState != PlayerController.State.Training)
        {
            waterPanel.color = new Color(1f, 0.3f, 0.3f, 0.12f);
            waterAlertTimer = 0f;
            return;
        }

        waterAlertTimer += Time.deltaTime;

        float t = (Mathf.Sin(waterAlertTimer * 6f) + 1f) * 0.5f;

        Color idle = new Color(1f, 1f, 1f, 0.08f);
        Color alert = new Color(1f, 0.2f, 0.2f, 0.16f);

        waterPanel.color = Color.Lerp(idle, alert, t);
    }

    private void UpdateWaterLevel()
    {
        water.fillAmount = GameManager.Instance.Water;

        if (GameManager.Instance.Water <= 0 && !isDyingOfThirst)
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
