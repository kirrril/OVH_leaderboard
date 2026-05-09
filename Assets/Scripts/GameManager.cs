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
    private bool isLosingHealth;
    public int currentScore = 0;

    public float water = 0.5f;

    public float legsTraining;
    public float chestTraining;
    public float backTraining;
    public float treadmillTraining;
    public float bikeTraining;
    public float jumpboxTraining;
    public float barbellTraining;
    public float chest1Training;
    public float chest2Training;
    public float dipsTraining;
    public float back1Training;
    public float back2Training;
    public float rowerTraining;
    public float extensionTraining;
    public float backBarbell1Training;
    public float pullUpsTraining;


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
        
        TrainingManagement();
        WaterManagement();
        FallingDownManagement();
    }

    void ResetGame()
    {
        health = 5;
        currentScore = 0;
        water = 0.5f;

        legsTraining = 0;
        chestTraining = 0;
        backTraining = 0;

        treadmillTraining = 0;
        bikeTraining = 0;
        jumpboxTraining = 0;
        barbellTraining = 0;
        chest1Training = 0;
        chest2Training = 0;
        dipsTraining = 0;
        back1Training = 0;
        back2Training = 0;
        rowerTraining = 0;
        extensionTraining = 0;
        backBarbell1Training = 0;
        pullUpsTraining = 0;
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

    public void TrainingManagement()
    {
        legsTraining = treadmillTraining + bikeTraining + jumpboxTraining;
        chestTraining = barbellTraining + chest1Training + chest2Training + dipsTraining;
        backTraining = back1Training + back2Training + rowerTraining + extensionTraining + backBarbell1Training + pullUpsTraining;


        // legsTraining = 1.05f;
        // chestTraining = 1.05f;
        // backTraining = 0.5f;
    }

    private void WaterManagement()
    {
        if (playerController.currentState == PlayerController.State.Training)
        {
            float waterLoss = Time.deltaTime / 20;
            water -= waterLoss;
        }

        if (water <= 0)
        {
            if (!isLosingHealth)
            {
                isLosingHealth = true;
                StartCoroutine(LoseHealthOfThirst());
            }
        }
    }

    private IEnumerator LoseHealthOfThirst()
    {
        yield return new WaitForSeconds(2f);
        LoseHealth();
        water = 0.5f;
        playerController.currentState = PlayerController.State.DyingOfThirst;
        isLosingHealth = false;
    }

    private void FallingDownManagement()
    {
        if (playerController.transform.position.y < -15f)
        {
            playerController.HandleComeBack();
            LoseHealth();
        }
    }

    public void YouWin()
    {
        StartCoroutine(YouWinTransitionCorout());
    }

    IEnumerator YouWinTransitionCorout()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("YouWinScene");
    }

    public void YouLoose()
    {
        SceneManager.LoadScene("YouLoseScene");
    }
}
