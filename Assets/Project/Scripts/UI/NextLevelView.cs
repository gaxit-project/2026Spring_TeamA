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

    private void Start()
    {
        // 初期状態は透明にしておく
        SetAlpha(panelImage, 0f);
        SetAlpha(gameClearText, 0f);
        SetAlpha(loadingText, 0f);
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
        string clearStr = "GAME CLEAR";
        string loadingStr = "Loading...";
        if (UIManager.Instance != null && UIManager.Instance.textData != null)
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

        // テキストをフェードイン
        if (targetText != null)
        {
            await targetText.DOFade(1.0f, textFadeDuration).AsyncWaitForCompletion();
        }
    }
}