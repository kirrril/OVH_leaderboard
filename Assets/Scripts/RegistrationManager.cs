using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameObject signInManager;
    [SerializeField] private GameObject registrationManager;

    [SerializeField] private TMP_Text registrationStatus;
    [SerializeField] private GameObject signInButton;
    [SerializeField] private Button playAsGuestButton;
    private string baseText;

    void Start()
    {
        signInButton.SetActive(false);
    }

    public void LaunchSignUp(string username, string email, string password)
    {
        StartCoroutine(SignUpAndWaitForVerification(username, email, password));
    }

    private IEnumerator SignUpAndWaitForVerification(string username, string email, string password)
    {
        baseText = "Processing registration";
        var loadingAnimatedText = StartCoroutine(LoadingAnimation());

        RegisterData data = new RegisterData
        {
            username = username,
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest www = UnityWebRequest.Post("https://darwinsgym.eu/register.php", json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                var res = JsonUtility.FromJson<ServerResponse>(response);

                if (res.status == "success")
                {
                    baseText = "Account created! Check your email.";

                    while (true)
                    {
                        yield return new WaitForSeconds(3f);

                        using var check = UnityWebRequest.Get($"https://darwinsgym.eu/check_verified.php?username={UnityWebRequest.EscapeURL(username)}");
                        yield return check.SendWebRequest();

                        if (check.result == UnityWebRequest.Result.Success)
                        {
                            var verifResult = JsonUtility.FromJson<ServerResponse>(check.downloadHandler.text);
                            if (verifResult.status == "verified")
                            {
                                StopCoroutine(loadingAnimatedText);
                                registrationStatus.text = $"Welcome {username}!\nAccount activated!\nSing in to play.";
                                signInButton.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    StopCoroutine(loadingAnimatedText);
                    registrationStatus.text = res.message;
                }
            }
            else
            {
                StopCoroutine(loadingAnimatedText);
                registrationStatus.text = "Connection error. Try again.";
            }
        }
    }

    public void OnSingInButtonClick()
    {
        StopAllCoroutines();
        signInManager.SetActive(true);
        registrationManager.SetActive(false);
    }

    public void OnPlayAsGuestButtonClick()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator LoadingAnimation()
    {
        while (true)
        {
            registrationStatus.text = baseText + "";
            yield return new WaitForSeconds(0.1f);
            registrationStatus.text = baseText + ".";
            yield return new WaitForSeconds(0.1f);
            registrationStatus.text = baseText + "..";
            yield return new WaitForSeconds(0.1f);
            registrationStatus.text = baseText + "...";
            yield return new WaitForSeconds(0.1f);
        }
    }

    [System.Serializable]
    private class RegisterData
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    private class ServerResponse
    {
        public string status;
        public string message;
    }
}
