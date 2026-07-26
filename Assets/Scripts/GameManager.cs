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
    public PlayerTrainingType CurrentTrainingType { get; private set; }
    private Coroutine currentTrainingCoroutine;
    public event Action CurrentLevelChanged;

    public enum DeathReason { None, VoidFall, Thirst, Fight, BarbellWeight }
    private bool isDeathSequenceActive;
    private GameSceneHUD gameSceneHUD;

    public bool IsDeathSequenceActive => isDeathSequenceActive;


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
        gameSceneHUD = FindFirstObjectByType<GameSceneHUD>();

        gameSceneLoaded = true;
        ResetGame();
    }

    /// <test setup>
    void Start()
    {
        LegsTraining = 1f;
        ChestTraining = 1f;
        BackTraining = 0.7f;
        CurrentScore = 80;
    }
    /// </test setup>
    /// 

    void ResetGame()
    {
        StopAllCoroutines();
        currentTrainingCoroutine = null;

        Health = 5;
        CurrentScore = 0;
        SetWater(0.5f);
        CurrentLevel = CurrentLevelZone.None;
        CurrentTrainingType = PlayerTrainingType.None;

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

    public void RequestDeath(DeathReason reason)
    {
        if (!gameSceneLoaded) return;
        if (playerController == null) return;
        if (isDeathSequenceActive) return;

        StartCoroutine(DeathSequenceCoroutine(reason));
    }

    private IEnumerator DeathSequenceCoroutine(DeathReason reason)
    {
        isDeathSequenceActive = true;

        TrainingStopped();

        if (gameSceneHUD != null)
        {
            yield return StartCoroutine(gameSceneHUD.FadeInDeathScreen(reason));
        }

        Health -= 1;
        SetWater(0.5f);

        if (Health < 1)
        {
            isDeathSequenceActive = false;
            YouLoose();
            yield break;
        }

        playerController.RespawnAtEntryPoint();

        yield return new WaitForSeconds(1.5f);

        if (gameSceneHUD != null)
        {
            yield return StartCoroutine(gameSceneHUD.FadeOutDeathScreen());
        }

        isDeathSequenceActive = false;
    }

    public void TrainingStarted(PlayerTrainingType type)
    {
        TrainingStopped();

        if (type == PlayerTrainingType.None) return;

        CurrentTrainingType = type;
        currentTrainingCoroutine = StartCoroutine(ProcessTraining(type));
    }

    public void TrainingStopped()
    {
        if (currentTrainingCoroutine == null) return;

        CurrentTrainingType = PlayerTrainingType.None;
        StopCoroutine(currentTrainingCoroutine);
        currentTrainingCoroutine = null;
    }

    private IEnumerator ProcessTraining(PlayerTrainingType type)
    {
        while (true)
        {
            WaterManagement();
            AddTrainingProgress(type, Time.deltaTime * 0.1f);
            yield return null;
        }
    }

    private void AddTrainingProgress(PlayerTrainingType type, float delta)
    {
        switch (type)
        {
            case PlayerTrainingType.Treadmill:
                TreadmillTraining = Mathf.Clamp01(TreadmillTraining + delta);
                break;

            case PlayerTrainingType.Bike:
                BikeTraining = Mathf.Clamp01(BikeTraining + delta);
                break;

            case PlayerTrainingType.JumpBox:
                JumpboxTraining = Mathf.Clamp01(JumpboxTraining + delta);
                break;

            case PlayerTrainingType.BenchPress:
                BenchPressTraining = Mathf.Clamp01(BenchPressTraining + delta);
                break;

            case PlayerTrainingType.PecFly:
                PecFlyTraining = Mathf.Clamp01(PecFlyTraining + delta);
                break;

            case PlayerTrainingType.Crossover:
                CrossoverTraining = Mathf.Clamp01(CrossoverTraining + delta);
                break;

            case PlayerTrainingType.Dips:
                DipsTraining = Mathf.Clamp01(DipsTraining + delta);
                break;

            case PlayerTrainingType.LatPull:
                LatPullTraining = Mathf.Clamp01(LatPullTraining + delta);
                break;

            case PlayerTrainingType.CableRow:
                CableRowTraining = Mathf.Clamp01(CableRowTraining + delta);
                break;

            case PlayerTrainingType.Rower:
                RowerTraining = Mathf.Clamp01(RowerTraining + delta);
                break;

            case PlayerTrainingType.BackExtension:
                BackExtensionTraining = Mathf.Clamp01(BackExtensionTraining + delta);
                break;

            case PlayerTrainingType.TBar:
                TBarTraining = Mathf.Clamp01(TBarTraining + delta);
                break;

            case PlayerTrainingType.PullUps:
                PullUpsTraining = Mathf.Clamp01(PullUpsTraining + delta);
                break;
        }

        UpdateTrainingTotals();
    }

    public bool IsTrainingCompleted(PlayerTrainingType type)
    {
        switch (type)
        {
            case PlayerTrainingType.Treadmill: return TreadmillTraining >= 1f;
            case PlayerTrainingType.Bike: return BikeTraining >= 1f;
            case PlayerTrainingType.JumpBox: return JumpboxTraining >= 1f;
            case PlayerTrainingType.BenchPress: return BenchPressTraining >= 1f;
            case PlayerTrainingType.PecFly: return PecFlyTraining >= 1f;
            case PlayerTrainingType.Crossover: return CrossoverTraining >= 1f;
            case PlayerTrainingType.Dips: return DipsTraining >= 1f;
            case PlayerTrainingType.LatPull: return LatPullTraining >= 1f;
            case PlayerTrainingType.CableRow: return CableRowTraining >= 1f;
            case PlayerTrainingType.Rower: return RowerTraining >= 1f;
            case PlayerTrainingType.BackExtension: return BackExtensionTraining >= 1f;
            case PlayerTrainingType.TBar: return TBarTraining >= 1f;
            case PlayerTrainingType.PullUps: return PullUpsTraining >= 1f;
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

        if (Water <= 0) RequestDeath(DeathReason.Thirst);
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
