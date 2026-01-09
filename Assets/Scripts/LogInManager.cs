using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameObject signInManager;
    [SerializeField] private GameObject signUpManager;
    [SerializeField] private GameObject logInManager;

    [SerializeField] private TMP_Text logInStatus;
    [SerializeField] private GameObject signInButton;
    [SerializeField] private GameObject signUpButton;
    [SerializeField] private Button playAsGuestButton;
    private string baseText;

    void Start()
    {
        signInButton.SetActive(false);
    }

    public void LaunchSignIn(string username, string password)
    {
        StartCoroutine(SignInCoroutine(username, password));
    }

    private IEnumerator SignInCoroutine(string username, string password)
    {
        baseText = "Logging in";

        var loadingAnimatedText = StartCoroutine(LoadingAnimation());

        LogInData data = new LogInData
        {
            player_name = username,
            password = password
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest www = UnityWebRequest.Post("https://darwinsgym.eu/login.php", json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                var res = JsonUtility.FromJson<LoginResponse>(response);

                if (res.status == "success")
                {
                    StopCoroutine(loadingAnimatedText);
                    baseText = "Successfully logged in!";

                    PlayerPrefs.SetString("SessionToken", res.session_token);
                    PlayerPrefs.SetString("PlayerName", res.player_name);
                    PlayerPrefs.Save();
                    GameManager.isCompeting = true;
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    StopCoroutine(loadingAnimatedText);
                    logInStatus.text = res.message ?? "Login failed";
                }
            }
            else
            {
                StopCoroutine(loadingAnimatedText);
                logInStatus.text = "Connection error. Try again.";
            }
        }
    }

    public void OnSignInButtonClick()
    {
        signInManager.SetActive(true);
        logInManager.SetActive(false);
    }

    public void OnPlayAsGuestButtonClick()
    {
        StopAllCoroutines();
        GameManager.isCompeting = false;
        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator LoadingAnimation()
    {
        while (true)
        {
            logInStatus.text = baseText + "";
            yield return new WaitForSeconds(0.1f);
            logInStatus.text = baseText + ".";
            yield return new WaitForSeconds(0.1f);
            logInStatus.text = baseText + "..";
            yield return new WaitForSeconds(0.1f);
            logInStatus.text = baseText + "...";
            yield return new WaitForSeconds(0.1f);
        }
    }

    [System.Serializable]
    private class LogInData
    {
        public string player_name;
        public string password;
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string status;
        public string player_name;
        public string session_token;
        public string message;
    }
}
