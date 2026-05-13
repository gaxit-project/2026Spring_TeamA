using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Slider settingsSlider;
    [SerializeField] private Button startButton;

    [Header("Text Labels")]
    [SerializeField] private TextMeshProUGUI gameTitleText;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI settingsButtonText;
    [SerializeField] private TextMeshProUGUI exitButtonText;

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
    }

    public void OpenSettings()
    {
        Debug.Log("Settingsを開きます");

        settingsPanel.SetActive(true);
        titlePanel.SetActive(false);


        //選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsSlider.gameObject);
    }

    public void CloseSettings()
    {
        Debug.Log("Settingsを閉じます");

        titlePanel.SetActive(true);
        settingsPanel.SetActive(false);

        //選択状態の変更
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
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
}
