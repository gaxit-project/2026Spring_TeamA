using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Slider settingsSlider;
    [SerializeField] private Button startButton;

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
