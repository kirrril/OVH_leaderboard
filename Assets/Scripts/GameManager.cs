using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject playerPrefab;
    private PlayerController playerController;

    private bool gameSceneLoaded;

    public int Health { get; private set; }
    private bool isLosingHealthOfThirst;
    public int CurrentScore { get; private set; }

    public float Water { get; private set; }

    public CurrentLevelZone CurrentLevel { get; private set; }
    public float LegsTraining { get; private set; }
    public float ChestTraining { get; private set; }
    public float BackTraining { get; private set; }
    public float TreadmillTraining { get; private set; }
    public float BikeTraining { get; private set; }
    public float JumpboxTraining { get; private set; }
    public float BenchPressTraining { get; private set; }
    public float PecFlyTraining { get; private set; }
    public float CrossoverTraining { get; private set; }
    public float DipsTraining { get; private set; }
    public float LatPullTraining { get; private set; }
    public float CableRowTraining { get; private set; }
    public float RowerTraining { get; private set; }
    public float BackExtensionTraining { get; private set; }
    public float TBarTraining { get; private set; }
    public float PullUpsTraining { get; private set; }
    public TrainingProgressType CurrentTrainingType { get; private set; }
    private Coroutine currentTrainingCoroutine;
    public event Action CurrentLevelChanged;


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

        Health = 5;
        CurrentScore = 0;
        SetWater(0.5f);
        CurrentLevel = CurrentLevelZone.None;
        CurrentTrainingType = TrainingProgressType.None;

        LegsTraining = 0;
        ChestTraining = 0;
        BackTraining = 0;

        TreadmillTraining = 0;
        BikeTraining = 0;
        JumpboxTraining = 0;
        BenchPressTraining = 0;
        PecFlyTraining = 0;
        CrossoverTraining = 0;
        DipsTraining = 0;
        LatPullTraining = 0;
        CableRowTraining = 0;
        RowerTraining = 0;
        BackExtensionTraining = 0;
        TBarTraining = 0;
        PullUpsTraining = 0;
    }

    public void ModifyScore(int delta)
    {
        CurrentScore = Mathf.Max(0, CurrentScore + delta);
    }

    public void LoseHealth()
    {
        Health -= 1;

        if (Health < 1) YouLoose();
    }

    private void SetWater(float value)
    {
        Water = Mathf.Clamp01(value);
    }

    private void ModifyWater(float delta)
    {
        SetWater(Water + delta);
    }

    public void RefillWater()
    {
        SetWater(1f);
    }

    public void SetCurrentLevel(CurrentLevelZone level)
    {
        if (CurrentLevel == level) return;

        CurrentLevel = level;
        CurrentLevelChanged?.Invoke();
    }

    public void TrainingStarted(TrainingProgressType type)
    {
        TrainingStopped();

        if (type == TrainingProgressType.None) return;

        CurrentTrainingType = type;
        currentTrainingCoroutine = StartCoroutine(ProcessTraining(type));
    }

    public void TrainingStopped()
    {
        if (currentTrainingCoroutine == null) return;

        CurrentTrainingType = TrainingProgressType.None;
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

            case TrainingProgressType.BenchPress:
                BenchPressTraining = Mathf.Clamp01(BenchPressTraining + delta);
                break;

            case TrainingProgressType.PecFly:
                PecFlyTraining = Mathf.Clamp01(PecFlyTraining + delta);
                break;

            case TrainingProgressType.Crossover:
                CrossoverTraining = Mathf.Clamp01(CrossoverTraining + delta);
                break;

            case TrainingProgressType.Dips:
                DipsTraining = Mathf.Clamp01(DipsTraining + delta);
                break;

            case TrainingProgressType.LatPull:
                LatPullTraining = Mathf.Clamp01(LatPullTraining + delta);
                break;

            case TrainingProgressType.CableRow:
                CableRowTraining = Mathf.Clamp01(CableRowTraining + delta);
                break;

            case TrainingProgressType.Rower:
                RowerTraining = Mathf.Clamp01(RowerTraining + delta);
                break;

            case TrainingProgressType.BackExtension:
                BackExtensionTraining = Mathf.Clamp01(BackExtensionTraining + delta);
                break;

            case TrainingProgressType.TBar:
                TBarTraining = Mathf.Clamp01(TBarTraining + delta);
                break;

            case TrainingProgressType.PullUps:
                PullUpsTraining = Mathf.Clamp01(PullUpsTraining + delta);
                break;
        }

        UpdateTrainingTotals();
    }

    public bool IsTrainingCompleted(TrainingProgressType type)
    {
        switch (type)
        {
            case TrainingProgressType.Treadmill: return TreadmillTraining >= 1f;
            case TrainingProgressType.Bike: return BikeTraining >= 1f;
            case TrainingProgressType.JumpBox: return JumpboxTraining >= 1f;
            case TrainingProgressType.BenchPress: return BenchPressTraining >= 1f;
            case TrainingProgressType.PecFly: return PecFlyTraining >= 1f;
            case TrainingProgressType.Crossover: return CrossoverTraining >= 1f;
            case TrainingProgressType.Dips: return DipsTraining >= 1f;
            case TrainingProgressType.LatPull: return LatPullTraining >= 1f;
            case TrainingProgressType.CableRow: return CableRowTraining >= 1f;
            case TrainingProgressType.Rower: return RowerTraining >= 1f;
            case TrainingProgressType.BackExtension: return BackExtensionTraining >= 1f;
            case TrainingProgressType.TBar: return TBarTraining >= 1f;
            case TrainingProgressType.PullUps: return PullUpsTraining >= 1f;
            default: return false;
        }
    }

    private void UpdateTrainingTotals()
    {
        LegsTraining = (TreadmillTraining + BikeTraining + JumpboxTraining) / 3f;
        ChestTraining = (BenchPressTraining + PecFlyTraining + CrossoverTraining + DipsTraining) / 4f;
        BackTraining = (LatPullTraining + CableRowTraining + RowerTraining + BackExtensionTraining + TBarTraining + PullUpsTraining) / 6f;
    }

    private void WaterManagement()
    {
        ModifyWater(-Time.deltaTime / 20f);

        if (Water <= 0)
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
        SetWater(0.5f);
        playerController.currentState = PlayerController.State.DyingOfThirst;
        isLosingHealthOfThirst = false;
    }

    private void FallingDownManagement()
    {
        if (playerController.transform.position.y < -15f)
        {
            playerController.HandleComeBack();
            LoseHealth();
            SetWater(0.5f);
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
