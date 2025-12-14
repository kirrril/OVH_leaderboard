using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboardBody;
    [SerializeField] private TMP_Text spanText;
    [SerializeField] private Button ascDescButton;
    [SerializeField] private GameObject asc;
    [SerializeField] private GameObject desc;
    [SerializeField] private GameObject showTop10;
    [SerializeField] private GameObject hideTop10;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] private Button top10Button;
    [SerializeField] private Button hideButton;
    private string publicURL = "https://darwinsgym.eu/";
    private string playerName;
    private string sessionToken;
    private int span = 0;
    private int totalPages;
    private bool isDesc = true;
    private string formattedPage;
    private int rank = 1;

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "");
        sessionToken = PlayerPrefs.GetString("SessionToken", "");
        DisplaySpan();
        GetTotalPages();
        DisplayLeaderboard();
    }

    public void OnAscDescClick()
    {
        isDesc = !isDesc;
        asc.SetActive(!isDesc);
        desc.SetActive(isDesc);
        DisplayLeaderboard();
    }

    public void OnForwardButtonClick()
    {
        span++;
        forwardButton.interactable = (span < totalPages - 1) ? true : false;
        backwardButton.interactable = (span > 0) ? true : false;
        DisplaySpan();
        DisplayLeaderboard();
    }

    public void OnBackwardButtonClick()
    {
        span--;
        forwardButton.interactable = (span < totalPages - 1) ? true : false;
        backwardButton.interactable = (span > 0) ? true : false;
        DisplaySpan();
        DisplayLeaderboard();
    }

    private void DisplaySpan()
    {
        if (span == 0)
        {
            spanText.text = "1\u201310";
        }
        else
        {
            spanText.text = $"{span}1\u2013{span + 1}0";
        }
    }

    public async void OnShowTop10ButtonClick()
    {
        await GetTop10Task();
        forwardButton.interactable = false;
        backwardButton.interactable = false;
        ascDescButton.interactable = false;
        hideTop10.SetActive(true);
        showTop10.SetActive(false);
        spanText.text = "TOP 10";
    }

    public async void OnHideTop10ButtonClick()
    {
        await GetLeaderboardPage();
        forwardButton.interactable = true;
        backwardButton.interactable = true;
        ascDescButton.interactable = true;
        showTop10.SetActive(true);
        hideTop10.SetActive(false);
        DisplaySpan();
    }

    public async Task GetTop10Task()
    {
        string url = $"{publicURL}get_top10.php";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 8;

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            leaderboardBody.text = "OFFLINE";
            return;
        }

        var response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
        formattedPage = "";
        rank = 1;

        foreach (var player in response.players)
        {
            string line;
            if (player.player_name == playerName)
            {
                line = $"<color=#ffdd92><b>{rank}.<pos=7%>{player.player_name}<pos=52%>{player.score}<pos=75%>{player.date}</b></color>\n\n";
            }
            else
            {
                line = $"<color=#ffffff>{rank}.<pos=7%>{player.player_name}<pos=52%>{player.score}<pos=75%>{player.date}</color>\n\n";
            }
            formattedPage += line;
            rank++;
        }
        leaderboardBody.text = formattedPage;
    }

    public void OnHideButtonClick()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameScene");
    }

    private async void DisplayLeaderboard()
    {
        await GetLeaderboardPage();
    }

    private async Task GetLeaderboardPage()
    {
        string url = $"{publicURL}get_page.php?page={span}&is_desc={isDesc.ToString().ToLower()}";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 8;

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            leaderboardBody.text = "OFFLINE";
            return;
        }

        var response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
        formattedPage = "";
        rank = span * 10 + 1;

        foreach (var player in response.players)
        {
            string line;
            if (player.player_name == playerName)
            {
                line = $"<color=#ffdd92><b>{rank}.<pos=7%>{player.player_name}<pos=52%>{player.score}<pos=75%>{player.date}</b></color>\n\n";
            }
            else
            {
                line = $"<color=#ffffff>{rank}.<pos=7%>{player.player_name}<pos=52%>{player.score}<pos=75%>{player.date}</color>\n\n";
            }
            formattedPage += line;
            rank++;
        }
        leaderboardBody.text = formattedPage;
    }

    private async void GetTotalPages()
    {
        await GetTotalPagesTask();
        Debug.Log(totalPages);
    }

    private async Task GetTotalPagesTask()
    {
        string url = $"{publicURL}get_total_pages.php";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 8;

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<TotalPagesResponse>(www.downloadHandler.text);

            totalPages = response.total_pages;
        }
    }

    [System.Serializable]
    private class TotalPagesResponse
    {
        public int total_pages;
    }

    [System.Serializable]
    private class LeaderboardResponse
    {
        public PlayerData[] players;
    }

    [System.Serializable]
    private class PlayerData
    {
        public string player_name;
        public int score;
        public string date;
    }
}