using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject playerPrefab;
    private PlayerController playerController;

    private bool gameSceneLoaded;

    public int health = 5;
    private bool isLosingHealthOfThirst;
    public int currentScore = 0;

    public float water = 0.5f;

    public CurrentLevelZone CurrentLevel { get; private set; }
    public float LegsTraining { get; private set; }
    public float ChestTraining { get; private set; }
    public float BackTraining { get; private set; }
    public float TreadmillTraining { get; private set; }
    public float BikeTraining { get; private set; }
    public float JumpboxTraining { get; private set; }
    public float BarbellTraining { get; private set; }
    public float ChestMachine1Training { get; private set; }
    public float ChestMachine2Training { get; private set; }
    public float DipsTraining { get; private set; }
    public float BackMachine1Training { get; private set; }
    public float BackMachine2Training { get; private set; }
    public float RowerTraining { get; private set; }
    public float BackExtensionTraining { get; private set; }
    public float BackBarbell1Training { get; private set; }
    public float PullUpsTraining { get; private set; }
    private Coroutine currentTrainingCoroutine;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += HandleGameSceneLoaded;
        }
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleGameSceneLoaded;
        }
    }

    private void HandleGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameSceneLoaded = false;

        if (scene.name != "GameScene")
        {
            return;
        }

        playerPrefab = GameObject.Find("PlayerPrefab");
        if (playerPrefab == null) return;
        playerController = playerPrefab.GetComponent<PlayerController>();
        if (playerController == null) return;

        gameSceneLoaded = true;
        ResetGame();
    }

    void Update()
    {
        if (!gameSceneLoaded) return;

        FallingDownManagement();
    }

    void ResetGame()
    {
        StopAllCoroutines();
        currentTrainingCoroutine = null;
        isLosingHealthOfThirst = false;

        health = 5;
        currentScore = 0;
        water = 0.5f;
        CurrentLevel = CurrentLevelZone.None;

        LegsTraining = 0;
        ChestTraining = 0;
        BackTraining = 0;

        TreadmillTraining = 0;
        BikeTraining = 0;
        JumpboxTraining = 0;
        BarbellTraining = 0;
        ChestMachine1Training = 0;
        ChestMachine2Training = 0;
        DipsTraining = 0;
        BackMachine1Training = 0;
        BackMachine2Training = 0;
        RowerTraining = 0;
        BackExtensionTraining = 0;
        BackBarbell1Training = 0;
        PullUpsTraining = 0;
    }

    public void ModifyScore(int delta)
    {
        currentScore = Mathf.Max(0, currentScore + delta);
    }

    public void LoseHealth()
    {
        health -= 1;

        if (health < 1) YouLoose();
    }

    public void SetCurrentLevel(CurrentLevelZone level)
    {
        CurrentLevel = level;
    }

    public void TrainingStarted(TrainingProgressType type)
    {
        TrainingStopped();

        if (type == TrainingProgressType.None) return;

        currentTrainingCoroutine = StartCoroutine(ProcessTraining(type));
    }

    public void TrainingStopped()
    {
        if (currentTrainingCoroutine == null) return;

        StopCoroutine(currentTrainingCoroutine);
        currentTrainingCoroutine = null;
    }

    private IEnumerator ProcessTraining(TrainingProgressType type)
    {
        while (true)
        {
            WaterManagement();
            AddTrainingProgress(type, Time.deltaTime * 0.1f);
            yield return null;
        }
    }

    private void AddTrainingProgress(TrainingProgressType type, float delta)
    {
        switch (type)
        {
            case TrainingProgressType.Treadmill:
                TreadmillTraining = Mathf.Clamp01(TreadmillTraining + delta);
                break;

            case TrainingProgressType.Bike:
                BikeTraining = Mathf.Clamp01(BikeTraining + delta);
                break;

            case TrainingProgressType.JumpBox:
                JumpboxTraining = Mathf.Clamp01(JumpboxTraining + delta);
                break;

            case TrainingProgressType.Barbell:
                BarbellTraining = Mathf.Clamp01(BarbellTraining + delta);
                break;

            case TrainingProgressType.ChestMachine1:
                ChestMachine1Training = Mathf.Clamp01(ChestMachine1Training + delta);
                break;

            case TrainingProgressType.ChestMachine2:
                ChestMachine2Training = Mathf.Clamp01(ChestMachine2Training + delta);
                break;

            case TrainingProgressType.Dips:
                DipsTraining = Mathf.Clamp01(DipsTraining + delta);
                break;

            case TrainingProgressType.BackMachine1:
                BackMachine1Training = Mathf.Clamp01(BackMachine1Training + delta);
                break;

            case TrainingProgressType.BackMachine2:
                BackMachine2Training = Mathf.Clamp01(BackMachine2Training + delta);
                break;

            case TrainingProgressType.Rower:
                RowerTraining = Mathf.Clamp01(RowerTraining + delta);
                break;

            case TrainingProgressType.BackExtension:
                BackExtensionTraining = Mathf.Clamp01(BackExtensionTraining + delta);
                break;

            case TrainingProgressType.BackBarbell1:
                BackBarbell1Training = Mathf.Clamp01(BackBarbell1Training + delta);
                break;

            case TrainingProgressType.PullUps:
                PullUpsTraining = Mathf.Clamp01(PullUpsTraining + delta);
                break;
        }

        UpdateTrainingTotals();
    }

    private void UpdateTrainingTotals()
    {
        LegsTraining = (TreadmillTraining + BikeTraining + JumpboxTraining) / 3f;
        ChestTraining = (BarbellTraining + ChestMachine1Training + ChestMachine2Training + DipsTraining) / 4f;
        BackTraining = (BackMachine1Training + BackMachine2Training + RowerTraining + BackExtensionTraining + BackBarbell1Training + PullUpsTraining) / 6f;
    }

    private void WaterManagement()
    {
        water = Mathf.Max(0f, water - Time.deltaTime / 20f);

        if (water <= 0)
        {
            if (!isLosingHealthOfThirst)
            {
                isLosingHealthOfThirst = true;
                StartCoroutine(LoseHealthOfThirst());
                TrainingStopped();
            }
        }
    }

    private IEnumerator LoseHealthOfThirst()
    {
        yield return new WaitForSeconds(2f);
        LoseHealth();
        water = 0.5f;
        playerController.currentState = PlayerController.State.DyingOfThirst;
        isLosingHealthOfThirst = false;
    }

    private void FallingDownManagement()
    {
        if (playerController.transform.position.y < -15f)
        {
            playerController.HandleComeBack();
            LoseHealth();
            water = 0.5f;
        }
    }

    public void YouWin()
    {
        SetCurrentLevel(CurrentLevelZone.None);
        StartCoroutine(YouWinTransitionCorout());
    }

    IEnumerator YouWinTransitionCorout()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("YouWinScene");
    }

    public void YouLoose()
    {
        SetCurrentLevel(CurrentLevelZone.None);
        SceneManager.LoadScene("YouLoseScene");
    }
}
