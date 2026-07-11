using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameSceneHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text currentScoreValue;
    [SerializeField] private GameObject bodyMap;
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
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Sprite[] healthPointsSprites;
    [SerializeField] private Image healthPanel;

    [SerializeField] private GameObject jumpChargeProgress;
    [SerializeField] private Image jumpChargeProgressFill;

    [SerializeField] private Image[] legsProgressBarPanels;
    [SerializeField] private Image[] chestProgressBarPanels;
    [SerializeField] private Image[] backProgressBarPanels;

    [SerializeField] private GameObject contextMessage;
    [SerializeField] private TMP_Text contextMessageText;

    [SerializeField] private Image deathScreen;

    private float waterAlertTimer;
    private float contextMessageTimer;


    void Update()
    {
        UpdateCurrentScoreValue();
        UpdateWaterLevel();
        ActivateWaterLevelAlert();
        UpdateHealthPoints();
        ActivateHealthAlert();
        UpdateTrainingProgress();
        ManageTrainingPanels();
        HighlightProgressBar();
        DisplayContextMessage();
        UpdateJumpChargeProgress();
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
                bodyMap.SetActive(true);
                legsTraining.SetActive(true);
                chestTraining.SetActive(false);
                backTraining.SetActive(false);
                ManageProgressOrCompleted(legsProgress, legsCompleted, GameManager.Instance.LegsTraining);
                ManageProgressOrCompleted(treadmillProgress, treadmillCompleted, GameManager.Instance.TreadmillTraining);
                ManageProgressOrCompleted(bikeProgress, bikeCompleted, GameManager.Instance.BikeTraining);
                ManageProgressOrCompleted(jumpboxProgress, jumpboxCompleted, GameManager.Instance.JumpboxTraining);
                break;
            case CurrentLevelZone.Chest:
                bodyMap.SetActive(true);
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
                bodyMap.SetActive(true);
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
                bodyMap.SetActive(false);
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
                    case TrainingType.Treadmill:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.Bike:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.JumpBox:
                        foreach (Image panel in legsProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        legsProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.None:
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
                    case TrainingType.BenchPress:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.PecFly:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.Crossover:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.Dips:
                        foreach (Image panel in chestProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        chestProgressBarPanels[3].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.None:
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
                    case TrainingType.LatPull:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[0].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.CableRow:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[1].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.Rower:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[2].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.BackExtension:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[3].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.TBar:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[4].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.PullUps:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        backProgressBarPanels[5].color = new Color(0, 0, 0, 0.3f);
                        break;
                    case TrainingType.None:
                        foreach (Image panel in backProgressBarPanels)
                        {
                            panel.color = new Color(0, 0, 0, 0f);
                        }
                        break;
                }
                break;
        }
    }

    private void ActivateWaterLevelAlert()
    {
        if (GameManager.Instance.Water > 0.4f)
        {
            waterPanel.color = new Color(1f, 1f, 1f, 0.08f);
            waterAlertTimer = 0f;
            return;
        }

        if (playerController.CurrentState != PlayerController.State.Training)
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

    private void UpdateJumpChargeProgress()
    {
        if (playerController.CurrentJumpZone == null
        || playerController.CurrentJumpZone.jumpType != JumpZone.JumpType.Charged
        || playerController.jumpPhase != PlayerController.JumpPhase.Charging)
        {
            jumpChargeProgressFill.fillAmount = 0;
            jumpChargeProgress.SetActive(false);
            return;
        }

        jumpChargeProgress.SetActive(true);
        jumpChargeProgressFill.fillAmount = playerController.JumpChargeCoeff;
    }

    private void DisplayContextMessage()
    {
        if (playerController.CurrentState == PlayerController.State.Training
            && playerController.CurrentTrainingType != TrainingType.None
            && GameManager.Instance.IsTrainingCompleted(playerController.CurrentTrainingType))
        {
            contextMessageText.text = GetTrainingCompletedAlertMessage(playerController.CurrentTrainingType);
            MakeTrainingAlertBlink();
        }
        else if (playerController.CurrentState == PlayerController.State.Walking
            && playerController.CurrentJumpZone != null)
        {
            if (playerController.CurrentJumpZone.jumpType == JumpZone.JumpType.Plain)
            {
                contextMessageText.text = "Press Space to jump";
            }
            if (playerController.CurrentJumpZone.jumpType == JumpZone.JumpType.Charged)
            {
                contextMessageText.text = "Hold Space to charge jump";
            }
        }
        else if (playerController.CurrentState == PlayerController.State.Jumping
            && playerController.jumpPhase == PlayerController.JumpPhase.Charging)
        {
            contextMessageText.text = "Release Space to jump";
        }
        else if (playerController.CurrentState == PlayerController.State.Walking
            && playerController.CurrentDoor != null)
        {
            contextMessageText.text = "Press Space to push";
        }
        else if (playerController.CurrentState == PlayerController.State.Walking
            && playerController.CurrentPole != null)
        {
            contextMessageText.text = "Press Space to climb";
        }
        else
        {
            contextMessageText.text = "";
            contextMessageText.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void MakeTrainingAlertBlink()
    {
        float t = (Mathf.Sin(contextMessageTimer * 6f) + 1f) * 0.5f;

        contextMessageTimer += Time.deltaTime;

        Color idle = new Color(1f, 1f, 1f, 1f);
        Color alert = new Color(1f, 1f, 1f, 0f);

        contextMessageText.color = Color.Lerp(idle, alert, t);
    }

    private string GetTrainingCompletedAlertMessage(TrainingType type)
    {
        switch (type)
        {
            case TrainingType.Treadmill:
                return "Treadmill training completed";
            case TrainingType.Bike:
                return "Bike training completed";
            case TrainingType.JumpBox:
                return "Jump box training completed";
            case TrainingType.BenchPress:
                return "Bench press training completed";
            case TrainingType.PecFly:
                return "Pec fly training completed";
            case TrainingType.Crossover:
                return "Crossover training completed";
            case TrainingType.Dips:
                return "Dips training completed";
            case TrainingType.LatPull:
                return "Lat pull training completed";
            case TrainingType.CableRow:
                return "Cable row training completed";
            case TrainingType.Rower:
                return "Rower training completed";
            case TrainingType.BackExtension:
                return "Back extension training completed";
            case TrainingType.TBar:
                return "T-bar training completed";
            case TrainingType.PullUps:
                return "Pull-ups training completed";
            default:
                return "Training completed";
        }
    }

    private void UpdateWaterLevel()
    {
        water.fillAmount = GameManager.Instance.Water;
    }

    private Color GetDeathColor(GameManager.DeathReason reason)
    {
        switch (reason)
        {
            case GameManager.DeathReason.Thirst:
                return new Color(1f, 1f, 1f, 1f);

            case GameManager.DeathReason.VoidFall:
                return new Color(0f, 0f, 0f, 1f);

            case GameManager.DeathReason.Fight:
                return new Color(0.4f, 0f, 0f, 1f);

            case GameManager.DeathReason.BarbellWeight:
                return new Color(0.2f, 0f, 0f, 1f);

            default:
                return new Color(0f, 0f, 0f, 1f);
        }
    }

    private float GetFadeInDuration(GameManager.DeathReason reason)
    {
        switch (reason)
        {
            case GameManager.DeathReason.Thirst:
                return 1.5f;

            case GameManager.DeathReason.VoidFall:
                return 1f;

            case GameManager.DeathReason.Fight:
                return 0.6f;

            case GameManager.DeathReason.BarbellWeight:
                return 1.5f;

            default:
                return 0.6f;
        }
    }

    public IEnumerator FadeInDeathScreen(GameManager.DeathReason reason)
    {
        float duration = GetFadeInDuration(reason);
        float elapsedTime = 0f;

        deathScreen.gameObject.SetActive(true);

        Color color = GetDeathColor(reason);
        color.a = 0f;
        deathScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            deathScreen.color = color;
            yield return null;
        }

        color.a = 1f;
        deathScreen.color = color;
    }

    public IEnumerator FadeOutDeathScreen()
    {
        float duration = 0.6f;
        float elapsedTime = 0f;

        Color color = deathScreen.color;
        color.a = 1f;
        deathScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - elapsedTime / duration);
            deathScreen.color = color;
            yield return null;
        }

        color.a = 0f;
        deathScreen.color = color;
        deathScreen.gameObject.SetActive(false);
    }
}
