using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausePresenter : MonoBehaviour
{
    [SerializeField] private PauseView view;
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused = false;

    private void Start()
    {
        view.OnResumeClicked += ResumeGame;
        view.OnSettingsClicked += OpenSettings;
        view.OnTitleClicked += GoToTitle;

        if (LanguageManager.Instance != null)
        {
            view.InitializeLabels(LanguageManager.Instance.CurrentTextData);
        }
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseActionPerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPauseActionPerformed;
        }
    }

    private void OnPauseActionPerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        view.Show();
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        view.Hide();

        UIEvents.OnPauseStateChanged?.Invoke(false);
    }

    private void OpenSettings()
    {
        Debug.Log("設定画面を開く");
    }

    private void GoToTitle()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Title");
    }
}
