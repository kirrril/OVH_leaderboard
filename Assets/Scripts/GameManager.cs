using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;
        if (PlayerData.Instance == null) return;

        ResetGame();
    }

    void Update()
    {
        TrainingManagement();
    }

    void ResetGame()
    {
        PlayerData.Instance.health = 5;
        PlayerData.Instance.currentScore = 0;

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
        PlayerData.Instance.currentScore = Mathf.Max(0, PlayerData.Instance.currentScore + delta);
    }

    public void LoseHealth()
    {
        PlayerData.Instance.health -= 1;

        if (PlayerData.Instance.health < 1) YouLoose();
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
