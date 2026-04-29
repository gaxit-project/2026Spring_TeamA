using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks; 

public class LanguageSelectionView : MonoBehaviour
{
    [SerializeField] private Button japaneseButton;
    [SerializeField] private Button englishButton;
    [SerializeField] private GameObject selectionPanel;

    [SerializeField] private string nextSceneName = "Main"; // 言語選択後に遷移するシーン名

    [SerializeField] private CanvasGroup titleMenuCanvasGroup; // タイトルのボタン群をまとめて操作する

    [SerializeField] private GameObject firstSelectedButton; // 開いた時に最初に選ぶボタン

    [SerializeField] private NextLevelView nextLevelView; // フェードアウト用


    private void Start()
    {
        japaneseButton.onClick.AddListener(() =>
        {
            LanguageManager.Instance.SetLanguage(true);
            StartGameAsync().Forget();
        });

        englishButton.onClick.AddListener(() =>
        {
            LanguageManager.Instance.SetLanguage(false);
            StartGameAsync().Forget();
        });

        if (selectionPanel != null) selectionPanel.SetActive(false);
    }

    public void OpenLanguageSelectionPanel()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);

            // 裏にあるタイトルのボタンを選択できないようにする
            if (titleMenuCanvasGroup != null)
            {
                titleMenuCanvasGroup.interactable = false;
                titleMenuCanvasGroup.blocksRaycasts = false;
            }

            // デフォルトのボタンを選択状態にする
            if (firstSelectedButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }

    private async UniTaskVoid StartGameAsync()
    {
        // ロード画面を開始し、真っ暗になるまで待つ
        if (nextLevelView != null)
        {
            nextLevelView.gameObject.SetActive(true);
            // "Loading..." の文字を表示する
            await nextLevelView.PlaySequence(false);
        }
        // 画面が真っ暗になったら裏側でパネルを消す
        if (selectionPanel != null) selectionPanel.SetActive(false);

        // シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }
}
