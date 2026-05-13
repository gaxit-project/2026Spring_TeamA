using System;
using TMPro;
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

    [Header("Button Text Labels")]
    [SerializeField] private TextMeshProUGUI resumeText;
    [SerializeField] private TextMeshProUGUI settingsText;
    [SerializeField] private TextMeshProUGUI quitText;

    /// <summary>
    /// 言語設定データに基づいて、ポーズ画面の各ボタンテキストを設定する
    /// </summary>
    public void InitializeLabels(TextData data)
    {
        if (data == null) return;

        if (resumeText != null) resumeText.text = data.pauseResumeButton;
        if (settingsText != null) settingsText.text = data.pauseSettingsButton;
        if (quitText != null) quitText.text = data.pauseQuitButton;
    }

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
