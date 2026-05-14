using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Button startButton;

    [Header("設定パネル")]
    [SerializeField] private GameObject settingsPanel;

    [Header("詳細設定パネル")]
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject languageSettingsPanel;

    [Header("設定パネルのボタン")]
    [SerializeField] private Button audioSettingsOpenButton;
    [SerializeField] private Button languageSettingsOpenButton;
    [SerializeField] private Button defaultLanguageButton;

    [Header("音量設定のスライダー")]
    [SerializeField] private Slider settingsSlider;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI gameTitleText;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI settingsButtonText;
    [SerializeField] private TextMeshProUGUI exitButtonText;
    [SerializeField] private TextMeshProUGUI[] returnButtonTexts;

    private void Start()
    {
        if (LanguageManager.Instance != null && LanguageManager.Instance.CurrentTextData != null)
        {
            InitializeLabels(LanguageManager.Instance.CurrentTextData);
        }
    }

    /// <summary>
    /// 言語設定データに基づいて、タイトル画面の各テキストを設定する
    /// </summary>
    public void InitializeLabels(TextData data)
    {
        if (data == null) return;

        if (gameTitleText != null) gameTitleText.text = data.gameTitleName;
        if (startButtonText != null) startButtonText.text = data.titleStartButton;
        if (settingsButtonText != null) settingsButtonText.text = data.titleSettingsButton;
        if (exitButtonText != null) exitButtonText.text = data.titleExitButton;

        if (returnButtonTexts != null)
        {
            foreach (var returnText in returnButtonTexts)
            {
                if (returnText != null) returnText.text = data.titleReturnButton;
            }
        }
    }

    /// <summary>
    /// 設定パネルを開き、初期選択ボタンにフォーカスを設定する
    /// </summary>
    public void OpenSettings()
    {
        Debug.Log("Settingsを開きます");

        settingsPanel.SetActive(true);
        titlePanel.SetActive(false);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        if (audioSettingsOpenButton != null)
        {
            EventSystem.current.SetSelectedGameObject(audioSettingsOpenButton.gameObject);
        }
        else if (settingsSlider != null)
        {
            EventSystem.current.SetSelectedGameObject(settingsSlider.gameObject);
        }
    }

    /// <summary>
    /// 設定パネルを閉じ、タイトル画面に戻る
    /// </summary>
    public void CloseSettings()
    {
        Debug.Log("Settingsを閉じます");

        titlePanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(false);
        if (languageSettingsPanel != null) languageSettingsPanel.SetActive(false);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    /// <summary>
    /// Audio設定パネルを開く
    /// </summary>
    public void OpenAudioSettings()
    {
        Debug.Log("Audio Settingsを開きます");
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(true);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        if (settingsSlider != null)
        {
            EventSystem.current.SetSelectedGameObject(settingsSlider.gameObject);
        }
    }

    /// <summary>
    /// Audio設定パネルを閉じ、設定パネルに戻る
    /// </summary>
    public void CloseAudioSettings()
    {
        Debug.Log("Audio Settingsを閉じます");
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        if (audioSettingsOpenButton != null)
        {
            EventSystem.current.SetSelectedGameObject(audioSettingsOpenButton.gameObject);
        }
    }

    /// <summary>
    /// Language設定パネルを開く
    /// </summary>
    public void OpenLanguageSettings()
    {
        Debug.Log("Language Settingsを開きます");
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (languageSettingsPanel != null) languageSettingsPanel.SetActive(true);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        if (defaultLanguageButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultLanguageButton.gameObject);
        }
    }

    /// <summary>
    /// Language設定パネルを閉じ、設定パネルに戻る
    /// </summary>
    public void CloseLanguageSettings()
    {
        Debug.Log("Language Settingsを閉じます");
        if (languageSettingsPanel != null) languageSettingsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        // 選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        if (languageSettingsOpenButton != null)
        {
            EventSystem.current.SetSelectedGameObject(languageSettingsOpenButton.gameObject);
        }
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 言語を日本語に設定し、UIテキストを更新する
    /// </summary>
    public void SetJapanese()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.SetLanguage(true);
            InitializeLabels(LanguageManager.Instance.CurrentTextData);
        }
    }

    /// <summary>
    /// 言語を英語に設定し、UIテキストを更新する
    /// </summary>
    public void SetEnglish()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.SetLanguage(false);
            InitializeLabels(LanguageManager.Instance.CurrentTextData);
        }
    }
}
