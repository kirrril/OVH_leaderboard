using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_Text connectionStatus;
    [SerializeField] private GameObject authentificationManager;
    [SerializeField] private GameObject signInManager;
    [SerializeField] private GameObject signInButton;
    [SerializeField] private GameObject connectionManager;

    private void Awake()
    {
        // DeleteLocalUser();
    }

    private void Start()
    {
        StartCoroutine(CheckSessionAndConnect());
    }

    private IEnumerator CheckSessionAndConnect()
    {
        var loadingAnimatedText = StartCoroutine(LoadingAnimation("Connecting"));

        string sessionToken = PlayerPrefs.GetString("SessionToken", "");

        if (string.IsNullOrEmpty(sessionToken))
        {
            StopCoroutine(loadingAnimatedText);
            authentificationManager.SetActive(true);
            connectionManager.SetActive(false);
            yield break;
        }

        TokenRequest tokenRequest = new TokenRequest { token = sessionToken };
        string json = JsonUtility.ToJson(tokenRequest);
        using var www = UnityWebRequest.Post("https://darwinsgym.eu/validate_token.php", json, "application/json");
        www.timeout = 8;

        yield return www.SendWebRequest();

        StopCoroutine(loadingAnimatedText);

        if (www.result == UnityWebRequest.Result.Success)
        {
            var res = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);

            if (res.status == "success")
            {
                connectionStatus.text = $"Welcome back, {res.player_name}!";
                yield return new WaitForSeconds(1);
                GameManager.isCompeting = true;
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                PlayerPrefs.DeleteKey("SessionToken");
                PlayerPrefs.DeleteKey("PlayerName");
                connectionStatus.text = "Session expired. Sign in please.";
                signInButton.SetActive(true);
            }
        }
        else
        {
            connectionStatus.text = $"Connection failed. Play offline!";
            yield return new WaitForSeconds(1);
            GameManager.isCompeting = false;
            SceneManager.LoadScene("GameScene");
        }
    }

    private IEnumerator LoadingAnimation(string baseText)
    {
        while (true)
        {
            connectionStatus.text = baseText + "";
            yield return new WaitForSeconds(0.1f);
            connectionStatus.text = baseText + ".";
            yield return new WaitForSeconds(0.1f);
            connectionStatus.text = baseText + "..";
            yield return new WaitForSeconds(0.1f);
            connectionStatus.text = baseText + "...";
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnSignInButtonClick()
    {
        signInManager.SetActive(true);
        connectionManager.SetActive(false);
    }

    private void DeleteLocalUser()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class TokenRequest
    {
        public string token;
    }

    [System.Serializable]
    private class TokenResponse
    {
        public string status;
        public string player_name;
        public string message;
    }
}