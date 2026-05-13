using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;     // 倒した数 (例: "10")
    [SerializeField] private TextMeshProUGUI rescueCountText;   // 助けたNPCの数 (例: "3")
    [SerializeField] private TextMeshProUGUI vaccineScoreText;  // ワクチンボーナス/ペナルティ (例: "+20" / "-10")
    [SerializeField] private TextMeshProUGUI totalScoreText;    // 最終集計スコア (例: "23")
    [SerializeField] private TextMeshProUGUI rankText;          // 最終ランク (例: "S")

    /// <summary>
    /// 各項目の実績値、ワクチン結果、最終スコア、及びランクを画面に反映する
    /// </summary>
    public void UpdateScoreDisplay(int killCount, int rescueCount, int vaccineValue, int totalScore, string rankName)
    {
        // ゾンビ撃破数
        if (killCountText != null)
        {
            killCountText.text = $"{killCount}";
        }

        // 生存者救助数
        if (rescueCountText != null)
        {
            rescueCountText.text = $"{rescueCount}";
        }

        // ワクチンボーナス/ペナルティ
        if (vaccineScoreText != null)
        {
            vaccineScoreText.text = vaccineValue > 0 ? $"+{vaccineValue}" : $"{vaccineValue}";
        }

        // 最終集計スコア
        if (totalScoreText != null)
        {
            totalScoreText.text = $"{totalScore}";
        }

        // 最終ランク
        if (rankText != null)
        {
            rankText.text = rankName;
        }
    }
}
