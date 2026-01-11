using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreManager;
    [SerializeField] private TMP_InputField scoreInput;
    [SerializeField] private TMP_Text scoreStatusText;
    [SerializeField] private GameObject rankManager;
    [SerializeField] private GameObject offlineStatusManager;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private Image upAndDown;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;

    [SerializeField] private TMP_Text highestScoreText;
    [SerializeField] private TMP_Text sessionScoreText;
    [SerializeField] private Button showLeaderboardButton;

    public static bool isCompeting;
    private bool isGuest;
    private string publicURL = "https://darwinsgym.eu/";
    private int highestScore;
    private int sessionScore;
    private int newRank;
    private int previousRank;
    private string playerName;
    private string sessionToken;

    void Start()
    {
        CheckStatus();
        GetHighestScore();
        StartCoroutine(UpdateRank());

        Debug.Log ($"{playerName}, {sessionToken}");
    }

    private void CheckStatus()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "");
        sessionToken = PlayerPrefs.GetString("SessionToken", "");
        if (!isCompeting)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                offlineStatusManager.SetActive(false);
                rankManager.SetActive(false);
            }
            else
            {
                offlineStatusManager.SetActive(true);
                rankManager.SetActive(false);
            }
        }
        else
        {
            offlineStatusManager.SetActive(false);
            rankManager.SetActive(true);
        }
    }

    private IEnumerator UpdateRank()
    {
        while (true)
        {
            GetRank();
            yield return new WaitForSeconds(3);
        }
    }

    private async void GetRank()
    {
        await GetRankTask();
    }

    private async Task GetRankTask()
    {
        NameToken nameToken = new NameToken { player_name = playerName, token = sessionToken };
        string json = JsonUtility.ToJson(nameToken);
        using var www = UnityWebRequest.Post("https://darwinsgym.eu/get_rank.php", json, "application/json");
        www.timeout = 8;

        await www.SendWebRequest();

        // using var www = new UnityWebRequest("https://darwinsgym.eu/get_rank.php", "POST");
        // byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        // www.uploadHandler = new UploadHandlerRaw(body);
        // www.downloadHandler = new DownloadHandlerBuffer();
        // www.SetRequestHeader("Content-Type", "application/json");
        // await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            var res = JsonUtility.FromJson<ServerResponse>(response);

            if (res.status == "success")
            {
                ProcessRank(res.rank);
            }
            else
            {
                rankText.text = "ERROR";
            }
        }
        else rankText.text = "OFFLINE";
    }

    private void ProcessRank(int rank)
    {
        previousRank = PlayerPrefs.GetInt("PreviousRank");
        newRank = rank;
        rankText.text = $"{newRank}";
        PlayerPrefs.SetInt("PreviousRank", rank);
        PlayerPrefs.Save();
        if (newRank != previousRank) upAndDown.sprite = (newRank <= previousRank) ? up : down;
    }

    public void OnScoreSubmission()
    {
        sessionScore = Convert.ToInt32(scoreInput.text);
        scoreInput.text = "";
        PostHighestScore();
        sessionScoreText.text = $"{sessionScore}";
    }

    private async void GetHighestScore()
    {
        await GetHighestScoreTask();
    }

    private async Task GetHighestScoreTask()
    {
        NameToken nameToken = new NameToken { player_name = playerName, token = sessionToken };
        string json = JsonUtility.ToJson(nameToken);
        // using var www = UnityWebRequest.Post("https://darwinsgym.eu/get_score.php", json, "application/json");
        // www.timeout = 8;

        // await www.SendWebRequest();

        using var www = new UnityWebRequest("https://darwinsgym.eu/get_score.php", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(body);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            var res = JsonUtility.FromJson<ServerResponse>(response);

            if (res.status == "success")
            {
                highestScore = res.score;
                highestScoreText.text = $"{highestScore}";
            }
            else
            {
                highestScoreText.text = "ERROR";
            }
        }
        else highestScoreText.text = "OFFLINE";
    }

    private async void PostHighestScore()
    {
        if (sessionScore > highestScore) await PostHighestScoreTask();
        GetHighestScore();
    }

    private async Task PostHighestScoreTask()
    {
        NewScore data = new NewScore { player_name = playerName, token = sessionToken, score = sessionScore };
        string json = JsonUtility.ToJson(data);
        // using var www = UnityWebRequest.Post("https://darwinsgym.eu/update_score.php", json, "application/json");
        // await www.SendWebRequest();

        using var www = new UnityWebRequest("https://darwinsgym.eu/update_score.php", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(body);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        await www.SendWebRequest();
    }

    public void OnShowLeaderboardClick()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("LeaderboardScene");
    }

    [System.Serializable]
    private class NewScore
    {
        public string player_name;
        public string token;
        public int score;
    }

    [System.Serializable]
    private class NameToken
    {
        public string player_name;
        public string token;
    }

    [System.Serializable]
    private class ServerResponse
    {
        public string status;
        public int score;
        public int rank;
        public string message;
    }
}
