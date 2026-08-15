using UnityEngine;
using UnityEngine.InputSystem;

public class CursorAndPauseController : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject sessionMenu;

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

        sessionMenu.SetActive(isPaused);
        ApplyCursorState();
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;

        sessionMenu.SetActive(isPaused);
        ApplyCursorState();
        Time.timeScale = 1;
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