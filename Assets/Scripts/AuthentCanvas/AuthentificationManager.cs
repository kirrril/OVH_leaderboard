using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthentificationManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameObject signUpManager;
    [SerializeField] private GameObject signInManager;
    [SerializeField] private GameObject authentManager;

    public void DisplaySignUpManager()
    {
        signUpManager.SetActive(true);
        signInManager.SetActive(false);
        authentManager.SetActive(false);
    }

    public void DisplaySignInManager()
    {
        signInManager.SetActive(true);
        signUpManager.SetActive(false);
        authentManager.SetActive(false);
    }

    public void SkipAuthentification()
    {
        SceneManager.LoadScene("GameScene");
    }
}
