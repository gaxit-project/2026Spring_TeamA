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

        int killScore = killCount * scoreData.killMultiplier;
        int rescueScore = rescueCount * scoreData.rescueMultiplier;
        int vaccineValue = isVaccineCleared ? scoreData.vaccineBonus : -scoreData.vaccinePenalty;
        int totalScore = killScore + rescueScore + vaccineValue;

        view.UpdateScoreDisplay(
            killCount, scoreData.killMultiplier, killScore,
            rescueCount, scoreData.rescueMultiplier, rescueScore,
            isVaccineCleared, vaccineValue,
            totalScore
        );
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}