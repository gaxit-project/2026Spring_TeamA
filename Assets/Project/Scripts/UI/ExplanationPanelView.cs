using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks; 

public class ExplanationPanelView : MonoBehaviour
{
    [SerializeField] private Button japaneseButton;
    [SerializeField] private Button englishButton;
    [SerializeField] private GameObject selectionPanel;

    [Header("説明用パネルリスト")]
    [SerializeField] private GameObject[] japaneseExplanationPages;
    [SerializeField] private GameObject[] englishExplanationPages;

    [SerializeField] private string nextSceneName = "Main"; // 言語選択後に遷移するシーン名

    [SerializeField] private CanvasGroup titleMenuCanvasGroup; // タイトルのボタン群をまとめて操作する

    [SerializeField] private NextLevelView nextLevelView; // フェードアウト用

    [Header("入力無視時間(秒)")]
    [SerializeField] private float ignoreInputDuration = 0.5f;

    private GameObject[] activePages;
    private int currentPageIndex = 0;
    private bool isStartingGame = false;
    private float stickCooldown = 0f;
    private float inputIgnoreTimer = 0f;
    private bool isPanelOpen = false;

    private void Start()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
    }

    /// <summary>
    /// シーン互換用の呼び出し口
    /// </summary>
    public void OpenLanguageSelectionPanel() => OpenExplanationPanel();

    /// <summary>
    /// スタートボタン押下時に呼ばれ、選択中の言語に応じた説明パネルの1ページ目を表示する
    /// </summary>
    public void OpenExplanationPanel()
    {
        isPanelOpen = true;

        if (titleMenuCanvasGroup != null)
        {
            titleMenuCanvasGroup.interactable = false;
            titleMenuCanvasGroup.blocksRaycasts = false;
        }

        bool isJapanese = true;
        if (LanguageManager.Instance != null)
        {
            isJapanese = LanguageManager.Instance.IsJapanese;
        }

        activePages = isJapanese ? japaneseExplanationPages : englishExplanationPages;
        currentPageIndex = 0;
        isStartingGame = false;
        stickCooldown = 0f;
        inputIgnoreTimer = ignoreInputDuration;

        UpdatePageDisplay();
    }

    /// <summary>
    /// 現在のページ表示を更新する
    /// </summary>
    private void UpdatePageDisplay()
    {
        if (activePages == null) return;

        if (japaneseExplanationPages != null)
        {
            foreach (var page in japaneseExplanationPages)
            {
                if (page != null) page.SetActive(false);
            }
        }
        if (englishExplanationPages != null)
        {
            foreach (var page in englishExplanationPages)
            {
                if (page != null) page.SetActive(false);
            }
        }

        if (currentPageIndex >= 0 && currentPageIndex < activePages.Length)
        {
            if (activePages[currentPageIndex] != null)
            {
                activePages[currentPageIndex].SetActive(true);
            }
        }
    }

    /// <summary>
    /// 左右入力や決定操作によるページ切り替えを監視する
    /// </summary>
    private void Update()
    {
        if (!isPanelOpen || isStartingGame) return;
        if (activePages == null || activePages.Length == 0) return;

        if (inputIgnoreTimer > 0f)
        {
            inputIgnoreTimer -= Time.deltaTime;
            return;
        }

        if (stickCooldown > 0f)
        {
            stickCooldown -= Time.deltaTime;
        }

        bool nextPage = false;
        bool prevPage = false;
        bool submit = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) submit = true;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) nextPage = true;
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) prevPage = true;
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) submit = true;
            if (Gamepad.current.dpad.right.wasPressedThisFrame) nextPage = true;
            if (Gamepad.current.dpad.left.wasPressedThisFrame) prevPage = true;

            if (stickCooldown <= 0f)
            {
                float stickX = Gamepad.current.leftStick.x.ReadValue();
                if (stickX > 0.5f)
                {
                    nextPage = true;
                    stickCooldown = 0.3f;
                }
                else if (stickX < -0.5f)
                {
                    prevPage = true;
                    stickCooldown = 0.3f;
                }
            }
        }

        if (submit)
        {
            if (currentPageIndex == activePages.Length - 1)
            {
                isStartingGame = true;
                StartGameAsync().Forget();
                return;
            }
        }

        if (nextPage && currentPageIndex < activePages.Length - 1)
        {
            currentPageIndex++;
            inputIgnoreTimer = ignoreInputDuration;
            UpdatePageDisplay();
        }
        else if (prevPage && currentPageIndex > 0)
        {
            currentPageIndex--;
            inputIgnoreTimer = ignoreInputDuration;
            UpdatePageDisplay();
        }
    }

    private async UniTaskVoid StartGameAsync()
    {
        isPanelOpen = false;

        // // 決定ボタン押下時に即座に説明用パネルを非表示にする
        // if (activePages != null && currentPageIndex >= 0 && currentPageIndex < activePages.Length)
        // {
        //     if (activePages[currentPageIndex] != null)
        //     {
        //         activePages[currentPageIndex].SetActive(false);
        //     }
        // }
        // if (selectionPanel != null) selectionPanel.SetActive(false);

        // ロード画面を表示し、シーケンスを開始する
        if (nextLevelView != null)
        {
            nextLevelView.gameObject.SetActive(true);
            // "Loading..." の文字を表示する
            await nextLevelView.PlaySequence(false);
        }

        // シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }
}
