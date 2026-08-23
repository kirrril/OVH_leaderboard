using UnityEngine;

public class AuthentCursorController : MonoBehaviour
{
    [SerializeField] private GameObject authentificationScreen;

    void OnEnable()
    {
        ApplyCursorState();
    }

    void Update()
    {
        ApplyCursorState();
    }

    void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ApplyCursorState()
    {
        bool showCursor = authentificationScreen != null && authentificationScreen.activeInHierarchy;

        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}