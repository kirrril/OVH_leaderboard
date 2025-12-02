using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpManager : MonoBehaviour
{
    [Header("Username")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text usernameStatus;
    [SerializeField] private Image usernameIcon;

    [Header("Email")]
    [SerializeField] private GameObject emailManager;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_Text emailStatus;
    [SerializeField] private Image emailIcon;

    [Header("Password")]
    [SerializeField] private GameObject passwordManager;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text passwordStatus;
    [SerializeField] private Image passwordIcon;

    [Header("Confirm Password")]
    [SerializeField] private GameObject confirmManager;
    [SerializeField] private TMP_InputField confirmInput;
    [SerializeField] private TMP_Text confirmStatus;
    [SerializeField] private Image confirmIcon;

    [Header("Sprites")]
    public Sprite checkSprite;
    public Sprite crossSprite;

    [Header("Button")]
    [SerializeField] private GameObject signUpButton;

    void Update()
    {
        DisplayHideElements();
    }

    private void DisplayHideElements()
    {
        emailManager.SetActive(ValidateUsername(usernameInput.text));
        passwordManager.SetActive(ValidateUsername(usernameInput.text) && ValidateEmail(emailInput.text));
        confirmManager.SetActive(ValidateUsername(usernameInput.text) && ValidateEmail(emailInput.text) && ValidatePassword(passwordInput.text));
        signUpButton.SetActive(IsSignupFormValid());
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

    private bool ValidateEmail(string email)
    {
        email = email.Trim();

        if (string.IsNullOrEmpty(email))
        {
            SetStatus(emailStatus, emailIcon, "Enter your email", false);
            return false;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            SetStatus(emailStatus, emailIcon, "Invalid email format", false);
            return false;
        }

        SetStatus(emailStatus, emailIcon, "Email OK", true);
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

    private bool ValidateConfirmPassword(string confirm, string original)
    {
        if (string.IsNullOrEmpty(confirm))
        {
            SetStatus(confirmStatus, confirmIcon, "Confirm your password", false);
            return false;
        }

        if (confirm != original)
        {
            SetStatus(confirmStatus, confirmIcon, "Passwords do not match", false);
            return false;
        }

        if (original.Length >= 8)
        {
            SetStatus(confirmStatus, confirmIcon, "Passwords match", true);
            return true;
        }

        return false;
    }

    private void SetStatus(TMP_Text text, Image icon, string message, bool valid)
    {
        text.text = message;
        icon.sprite = valid ? checkSprite : crossSprite;
    }

    private bool IsSignupFormValid()
    {
        return ValidateUsername(usernameInput.text) &&
               ValidateEmail(emailInput.text) &&
               ValidatePassword(passwordInput.text) &&
               ValidateConfirmPassword(confirmInput.text, passwordInput.text);
    }
}
