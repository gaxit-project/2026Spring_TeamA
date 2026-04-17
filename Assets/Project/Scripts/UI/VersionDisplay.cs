using UnityEngine;
using TMPro; // TextMeshProを使用

[RequireComponent(typeof(TextMeshProUGUI))]
public class VersionDisplay : MonoBehaviour
{
    [Tooltip("バージョンの前につける文字")]
    [SerializeField] private string prefix = "v";

    private void Start()
    {
        var textUI = GetComponent<TextMeshProUGUI>();

        // Application.version で Unityのプロジェクト設定からバージョン番号を取得
        textUI.text = prefix + Application.version;
    }
}
