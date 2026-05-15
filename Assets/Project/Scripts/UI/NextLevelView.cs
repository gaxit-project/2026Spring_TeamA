using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NextLevelView : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI gameClearText; // クリア時用のテキスト
    [SerializeField] private TextMeshProUGUI loadingText;   // ロード時用のテキスト
    [SerializeField] private float whiteoutDuration = 3.0f;
    [SerializeField] private float textFadeDuration = 1.0f;

    private System.Threading.CancellationTokenSource _loadingCts;

    private void Start()
    {
        // 初期状態は透明にしておく
        SetAlpha(panelImage, 0f);
        SetAlpha(gameClearText, 0f);
        SetAlpha(loadingText, 0f);
    }

    private void OnDestroy()
    {
        _loadingCts?.Cancel();
        _loadingCts?.Dispose();
        _loadingCts = null;
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
    }

    public async UniTask PlaySequence(bool isGameClear)
    {
        // UIManager経由でTextDataから文字を取得
        string clearStr = "ゲームクリア";
        string loadingStr = "ロード中...";
        
        // タイトル画面等、UIManagerが存在しない場合はLanguageManagerから取得
        if (LanguageManager.Instance != null && LanguageManager.Instance.CurrentTextData != null)
        {
            clearStr = LanguageManager.Instance.CurrentTextData.gameClearMessage;
            loadingStr = LanguageManager.Instance.CurrentTextData.loadingMessage;
        }
        else if (UIManager.Instance != null && UIManager.Instance.textData != null)
        {
            clearStr = UIManager.Instance.textData.gameClearMessage;
            loadingStr = UIManager.Instance.textData.loadingMessage;
        }

        TextMeshProUGUI targetText = null;

        // クリア時は白背景に黒文字、ロード時は黒背景に白文字を設定
        if (isGameClear)
        {
            if (panelImage != null) panelImage.color = new Color(1f, 1f, 1f, 0f);
            if (gameClearText != null)
            {
                gameClearText.color = new Color(0f, 0f, 0f, 0f); // 文字は黒
                gameClearText.text = clearStr;
                targetText = gameClearText;
            }
        }
        else
        {
            if (panelImage != null) panelImage.color = new Color(0f, 0f, 0f, 0f);
            if (loadingText != null)
            {
                loadingText.color = new Color(1f, 1f, 1f, 0f); // 文字は白
                loadingText.text = loadingStr;
                targetText = loadingText;
            }
        }

        // 使わない方のテキストを無効化し、使う方を有効化
        if (gameClearText != null) gameClearText.gameObject.SetActive(isGameClear);
        if (loadingText != null) loadingText.gameObject.SetActive(!isGameClear);

        // パネルをフェードイン
        if (panelImage != null)
        {
            await panelImage.DOFade(1.0f, whiteoutDuration).AsyncWaitForCompletion();
        }

        // 少し待つ
        await UniTask.Delay(System.TimeSpan.FromSeconds(1.0f));

        // Loadingアニメーションの開始
        if (!isGameClear && loadingText != null)
        {
            _loadingCts?.Cancel();
            _loadingCts?.Dispose();
            _loadingCts = new System.Threading.CancellationTokenSource();
            AnimateLoadingText(loadingStr, _loadingCts.Token).Forget();
        }

        // テキストをフェードイン
        if (targetText != null)
        {
            await targetText.DOFade(1.0f, textFadeDuration).AsyncWaitForCompletion();
        }
    }

    /// <summary>
    /// パネルとテキストをフェードアウトする非同期処理。
    /// </summary>
    public async UniTask FadeOutAsync()
    {
        // Loadingアニメーションの停止
        _loadingCts?.Cancel();
        _loadingCts?.Dispose();
        _loadingCts = null;

        // パネルとテキストをフェードアウト
        if (loadingText != null)
            await loadingText.DOFade(0f, textFadeDuration).AsyncWaitForCompletion();

        if (panelImage != null)
            await panelImage.DOFade(0f, whiteoutDuration).AsyncWaitForCompletion();
    }

    /// <summary>
    /// Loadingテキストのドットをアニメーションさせる非同期処理。
    /// </summary>
    private async UniTaskVoid AnimateLoadingText(string originalText, System.Threading.CancellationToken token)
    {
        string baseText = originalText.TrimEnd('.');
        int dotCount = 1;

        while (!token.IsCancellationRequested)
        {
            if (loadingText == null) break;

            string dots = new string('.', dotCount);
            loadingText.text = baseText + dots;

            // 0.5秒ごとに更新
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), cancellationToken: token).SuppressCancellationThrow();

            dotCount = (dotCount + 1) % 4; // 1, 2, 3, 0 のループ
        }
    }
}