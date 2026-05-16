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
        view.OnReturnClicked += CloseSettings;

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

        //一時的にポーズボタンを無効化
        pauseAction.action.Disable();
        view.OpenAudioSettings();
    }

    private void CloseSettings()
    {
        //ポーズボタンの再有効化
        pauseAction.action.Enable();
        view.CloseAudioSettings();
    }

    private void GoToTitle()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Title");
    }
}
