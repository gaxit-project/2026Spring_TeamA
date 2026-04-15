using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button returnTitleButton;
    [SerializeField] private float fadeDuration = 2.0f;

    private void Start()
    {
        SetAlpha(0);
        returnTitleButton.gameObject.SetActive(false); // ボタンは物理的に消しておく
    }

    private void SetAlpha(float alpha)
    {
        // Panelの色を変更
        Color c = panelImage.color;
        c.a = alpha;
        panelImage.color = c;

        // テキストの色も変更
        Color t = gameOverText.color;
        t.a = alpha;
        gameOverText.color = t;
    }

    public async UniTask PlayGameOverSequence()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
            await UniTask.Yield();
        }

        SetAlpha(1);
        returnTitleButton.gameObject.SetActive(true); // 最後にボタンを出す
    }
}