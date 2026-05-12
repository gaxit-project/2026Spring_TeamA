using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;        // 元の数値 (例: "10")
    [SerializeField] private TextMeshProUGUI killMultiplierText;   // 倍率の数値 (例: "*100")
    [SerializeField] private TextMeshProUGUI killScoreText;        // 計算後の数値 (例: "1000")
    
    [SerializeField] private TextMeshProUGUI rescueCountText;      // 元の数値 (例: "3")
    [SerializeField] private TextMeshProUGUI rescueMultiplierText; // 倍率の数値 (例: "*500")
    [SerializeField] private TextMeshProUGUI rescueScoreText;      // 計算後の数値 (例: "1500")

    [SerializeField] private TextMeshProUGUI vaccineScoreText;     // 計算後の数値 (例: "+2000" / "-1000")
    [SerializeField] private TextMeshProUGUI totalScoreText;       // 最終スコア (例: "2500")

    /// <summary>
    /// 倒したゾンビのカウント表示を更新
    /// </summary>
    public void UpdateKillCountDisplay(int count)
    {
        if (killCountText != null)
        {
            killCountText.text = $"{count}";
        }
    }

    /// <summary>
    /// 助けたNPCのカウント表示を更新
    /// </summary>
    public void UpdateRescueCountDisplay(int count)
    {
        if (rescueCountText != null)
        {
            rescueCountText.text = $"{count}";
        }
    }

    /// <summary>
    /// 各項目の数値、倍率、および計算結果スコアをテキスト表示に反映する
    /// </summary>
    public void UpdateScoreDisplay(int killCount, int killMultiplier, int killScore, int rescueCount, int rescueMultiplier, int rescueScore, bool isVaccineCleared, int vaccineValue, int totalScore)
    {
        // ゾンビ撃破の各テキスト更新
        if (killCountText != null)
        {
            killCountText.text = $"{killCount}";
        }
        if (killMultiplierText != null)
        {
            killMultiplierText.text = $"*{killMultiplier}";
        }
        if (killScoreText != null)
        {
            killScoreText.text = $"{killScore}";
        }

        // 生存者救助の各テキスト更新
        if (rescueCountText != null)
        {
            rescueCountText.text = $"{rescueCount}";
        }
        if (rescueMultiplierText != null)
        {
            rescueMultiplierText.text = $"*{rescueMultiplier}";
        }
        if (rescueScoreText != null)
        {
            rescueScoreText.text = $"{rescueScore}";
        }

        // ワクチン獲得のボーナス/ペナルティ結果
        if (vaccineScoreText != null)
        {
            vaccineScoreText.text = vaccineValue > 0 ? $"+{vaccineValue}" : $"{vaccineValue}";
        }

        // 最終スコア
        if (totalScoreText != null)
        {
            totalScoreText.text = $"{totalScore}";
        }
    }
}
