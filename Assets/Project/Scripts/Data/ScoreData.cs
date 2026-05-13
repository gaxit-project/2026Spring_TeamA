using UnityEngine;

[System.Serializable]
public class RankThreshold
{
    public string rankName; // ランク名（例: S, A, B, C）
    public int minScore;    // このランクになる最小スコア
}

[CreateAssetMenu(fileName = "ScoreData", menuName = "ScriptableObjects/ScoreData")]
public class ScoreData : ScriptableObject
{
    [Header("Vaccine Settings")]
    public int vaccinePenalty = 10; // ワクチン未回収ペナルティ値
    public int vaccineBonus = 20;   // ワクチン回収ボーナス値

    [Header("Rank Thresholds")]
    public RankThreshold[] rankThresholds;

    /// <summary>
    /// 合計スコアに基づいて該当するランク名を取得する
    /// </summary>
    public string EvaluateRank(int totalScore)
    {
        if (rankThresholds == null || rankThresholds.Length == 0)
        {
            return "N/A";
        }

        string selectedRank = "D"; // デフォルト
        int highestMatchMinScore = int.MinValue;

        foreach (var threshold in rankThresholds)
        {
            if (totalScore >= threshold.minScore && threshold.minScore > highestMatchMinScore)
            {
                highestMatchMinScore = threshold.minScore;
                selectedRank = threshold.rankName;
            }
        }

        return selectedRank;
    }
}
