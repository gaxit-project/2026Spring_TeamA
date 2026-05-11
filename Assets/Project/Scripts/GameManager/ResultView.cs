using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI rescueCountText;
    [SerializeField] private TextMeshProUGUI killScoreText;
    [SerializeField] private TextMeshProUGUI rescueScoreText;
    [SerializeField] private TextMeshProUGUI vaccineScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

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
    /// スコアの各項目と最終スコアの表示を更新する
    /// </summary>
    public void UpdateScoreDisplay(int killCount, int killMultiplier, int killScore, int rescueCount, int rescueMultiplier, int rescueScore, bool isVaccineCleared, int vaccineValue, int totalScore)
    {
        UpdateKillCountDisplay(killCount);
        UpdateRescueCountDisplay(rescueCount);

        if (killScoreText != null)
        {
            killScoreText.text = $"ゾンビ撃破: {killCount} × {killMultiplier} = {killScore}";
        }
        if (rescueScoreText != null)
        {
            rescueScoreText.text = $"生存者救助: {rescueCount} × {rescueMultiplier} = {rescueScore}";
        }
        if (vaccineScoreText != null)
        {
            if (isVaccineCleared)
            {
                vaccineScoreText.text = $"ワクチン回収ボーナス: +{vaccineValue}";
            }
            else
            {
                vaccineScoreText.text = $"ワクチン未回収ペナルティ: -{Mathf.Abs(vaccineValue)}";
            }
        }
        if (totalScoreText != null)
        {
            totalScoreText.text = $"最終スコア: {totalScore}";
        }
    }
}
