using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInManager : MonoBehaviour
{
    [Header("Username")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text usernameStatus;
    [SerializeField] private Image usernameIcon;

    [Header("Password")]
    [SerializeField] private GameObject passwordManager;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text passwordStatus;
    [SerializeField] private Image passwordIcon;

    [Header("Sprites")]
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;
    [SerializeField] private Sprite showSprite;
    [SerializeField] private Sprite hideSprite;
    [SerializeField] private Image passwordShowHide;

    [Header("Button")]
    [SerializeField] private GameObject signInButton;

    [Header("Managers")]
    [SerializeField] private GameObject signUpManager;
    [SerializeField] private GameObject signInManager;
    [SerializeField] private GameObject authentManager;
    [SerializeField] private GameObject logInManager;


    void Update()
    {
        DisplayHideElements();
    }

    private void DisplayHideElements()
    {
        passwordManager.SetActive(ValidateUsername(usernameInput.text));
        signInButton.SetActive(IsSignupFormValid());
    }

    private bool ValidateUsername(string username)
    {
        username = username.Trim();

        if (string.IsNullOrEmpty(username))
        {
            SetStatus(usernameStatus, usernameIcon, "Type your username", false);
            return false;
        }

        if (username.Length < 3 || username.Length > 20)
        {
            SetStatus(usernameStatus, usernameIcon, "3 to 20 characters", false);
            return false;
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
        {
            SetStatus(usernameStatus, usernameIcon, "Only letters, numbers, - or _", false);
            return false;
        }

        SetStatus(usernameStatus, usernameIcon, "Username OK", true);
        return true;
    }

    private bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            SetStatus(passwordStatus, passwordIcon, "Enter a password", false);
            return false;
        }

        if (password.Length < 8)
        {
            SetStatus(passwordStatus, passwordIcon, "Minimum 8 characters", false);
            return false;
        }

        bool hasLetter = Regex.IsMatch(password, @"[a-zA-Z]");
        bool hasDigit = Regex.IsMatch(password, @"[0-9]");

        if (!hasLetter || !hasDigit)
        {
            SetStatus(passwordStatus, passwordIcon, "Must contain letters + numbers", false);
            return false;
        }

        SetStatus(passwordStatus, passwordIcon, "Password strong", true);
        return true;
    }

    private void SetStatus(TMP_Text text, Image icon, string message, bool valid)
    {
        text.text = message;
        icon.sprite = valid ? checkSprite : crossSprite;
    }

    private bool IsSignupFormValid()
    {
        return ValidateUsername(usernameInput.text) &&
               ValidatePassword(passwordInput.text);
    }

    public void ShowHidePassword(TMP_InputField inputField)
    {
        bool wasPassword = inputField.contentType == TMP_InputField.ContentType.Password;
        inputField.contentType = wasPassword ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;

        inputField.DeactivateInputField();
        inputField.ActivateInputField();
        inputField.caretPosition = inputField.text.Length;

        passwordShowHide.sprite = wasPassword ? showSprite : hideSprite;
    }

    public void DisplaySignUpManager()
    {
        signUpManager.SetActive(true);
        signInManager.SetActive(false);
        authentManager.SetActive(false);
    }

    public void OnSignInButtonClick()
    {
        logInManager.SetActive(true);
        logInManager.GetComponent<LogInManager>().LaunchSignIn(usernameInput.text.Trim(), passwordInput.text.Trim());
        signInManager.SetActive(false);
    }

    public void SkipAuthentification()
    {
        GameManager.isCompeting = false;
        SceneManager.LoadScene("GameScene");
    }
}
