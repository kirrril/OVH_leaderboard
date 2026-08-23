using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CursorAndSessionMenuController : MonoBehaviour
{
    /////////////////////////////// GAME SCENE UNIQUEMENT ///////////////////////////////
    private bool isPaused;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject sessionOverlay;
    [SerializeField] private GameObject sessionMenu;
    [SerializeField] private GameObject restartMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] private GameObject soundMenu;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject howToPlayPanel;

    void OnEnable()
    {
        isPaused = false;
        Time.timeScale = 1;
        sessionOverlay.SetActive(false);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    void OnDisable()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    void Update()
    {
        ApplyCursorState();
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PauseAndResumeGame();
        }
    }

    private void PauseAndResumeGame()
    {
        isPaused = !isPaused;

        sessionOverlay.SetActive(isPaused);
        sessionMenu.SetActive(isPaused);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        ApplyCursorState();
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;

        sessionOverlay.SetActive(isPaused);
        sessionMenu.SetActive(isPaused);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        ApplyCursorState();
        Time.timeScale = 1;
    }

    public void OnRestartButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(true);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(false);
        quitMenu.SetActive(true);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    public void QuitGame()
    {
        
    }

    public void OnBackButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(true);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    public void OnSoundButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(true);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    public void OnControlsButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
    }

    public void OnHowToPlayButtonClick()
    {
        sessionOverlay.SetActive(true);
        sessionMenu.SetActive(false);
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void RestartSession()
    {
        isPaused = false;
        sessionOverlay.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    private CursorLockMode SetCursorLockState()
    {
        if (isPaused) return CursorLockMode.None;
        if (!isPaused && playerController.CurrentState == PlayerController.State.Training) return CursorLockMode.None;
        return CursorLockMode.Locked;
    }

    private bool SetCursorVisible()
    {
        if (isPaused) return true;
        if (!isPaused && playerController.CurrentState == PlayerController.State.Training) return true;
        return false;
    }

    private void ApplyCursorState()
    {
        Cursor.lockState = SetCursorLockState();
        Cursor.visible = SetCursorVisible();
    }
}