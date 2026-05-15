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
        bool isGameClear = SessionData.IsGameClear;
        int remainingSeconds = Mathf.FloorToInt(SessionData.RemainingTime);

        // ゾンビを倒した数、NPC救出数、ワクチンのボーナス/ペナルティ、残り時間を合計
        // ※ワクチンボーナスはゲームクリア時のみ適用し、ゲームオーバー時はペナルティ扱い（または適用外）とする
        int vaccineValue = (isGameClear && isVaccineCleared) ? scoreData.vaccineBonus : -scoreData.vaccinePenalty;
        int totalScore = killCount + rescueCount + vaccineValue + remainingSeconds;

        // 合計スコアを元にランクを算出（ゲームオーバー時は"FAILED"にする）
        string rankName = isGameClear ? scoreData.EvaluateRank(totalScore) : "FAILED";

        view.UpdateScoreDisplay(
            killCount,
            rescueCount,
            vaccineValue,
            remainingSeconds,
            totalScore,
            rankName
        );

        // ラベル表示言語の初期化
        if (LanguageManager.Instance != null)
        {
            view.InitializeLabels(LanguageManager.Instance.CurrentTextData, isVaccineCleared);
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}