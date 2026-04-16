using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class NextLevelView : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI ComingSoonText;
    [SerializeField] private float fadeDuration = 2.0f;

    private void Start()
    {
        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        // Panelの色を変更
        Color c = panelImage.color;
        c.a = alpha;
        panelImage.color = c;

        // テキストの色も変更
        Color t = ComingSoonText.color;
        t.a = alpha;
        ComingSoonText.color = t;
    }

    public async UniTask PlayComingSoonSequence()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
            await UniTask.Yield();
        }

        SetAlpha(1);
    }
}