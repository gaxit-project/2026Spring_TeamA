using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private ResultView view;

    [SerializeField] private ScoreData scoreData;

    /// <summary>
    /// 初期化時にスコア計算を行い、結果を表示に反映する
    /// </summary>
    private void Start()
    {
        if (scoreData == null)
        {
            Debug.LogError("ScoreData ScriptableObject is not assigned to ResultPresenter!");
            return;
        }

        int killCount = SessionData.KillCount;
        int rescueCount = SessionData.RescueCount;
        bool isVaccineCleared = SessionData.IsVaccineCleared;

        // ゾンビを倒した数、NPC救出数、ワクチンのボーナス/ペナルティを合計
        int vaccineValue = isVaccineCleared ? scoreData.vaccineBonus : -scoreData.vaccinePenalty;
        int totalScore = killCount + rescueCount + vaccineValue;

        // 合計スコアを元にランクを算出
        string rankName = scoreData.EvaluateRank(totalScore);

        view.UpdateScoreDisplay(
            killCount,
            rescueCount,
            vaccineValue,
            totalScore,
            rankName
        );
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}