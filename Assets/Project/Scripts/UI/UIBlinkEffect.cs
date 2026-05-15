using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 選択中(Selected)のボタンの透明度を点滅させるコンポーネント
/// </summary>
[RequireComponent(typeof(Button))]
public class UIBlinkEffect : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float minAlpha = 0.2f;

    private Button button;
    private Color originalSelectedColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalSelectedColor = button.colors.selectedColor;
    }

    private void Update()
    {
        // 常にSelectedの透明度をPingPongで変化させる
        float alpha = Mathf.Lerp(minAlpha, originalSelectedColor.a, Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f));
        
        ColorBlock colors = button.colors;
        colors.selectedColor = new Color(originalSelectedColor.r, originalSelectedColor.g, originalSelectedColor.b, alpha);
        button.colors = colors;
    }
}
