using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject defaultSelectedButton;

    public event Action OnResumeClicked;
    public event Action OnSettingsClicked;
    public event Action OnTitleClicked;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // ボタンのクリックイベントを紐付け
        resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
        settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());
        quitButton.onClick.AddListener(() => OnTitleClicked?.Invoke());

        Hide();
    }

    public void Show()
    {
        pausePanel.SetActive(true);

        if (defaultSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    public void Hide()
    {
        pausePanel.SetActive(false);
    }
}
