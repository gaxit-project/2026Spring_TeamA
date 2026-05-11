using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private ResultView view;

    [Header("Score Multipliers")]
    [SerializeField] private int killMultiplier = 100;     // n1
    [SerializeField] private int rescueMultiplier = 500;   // n2
    [SerializeField] private int vaccinePenalty = 1000;    // n3
    [SerializeField] private int vaccineBonus = 2000;      // n4

    /// <summary>
    /// 初期化時にスコア計算を行い、結果を表示に反映する
    /// </summary>
    private void Start()
    {
        int killCount = SessionData.KillCount;
        int rescueCount = SessionData.RescueCount;
        bool isVaccineCleared = SessionData.IsVaccineCleared;

        int killScore = killCount * killMultiplier;
        int rescueScore = rescueCount * rescueMultiplier;
        int vaccineValue = isVaccineCleared ? vaccineBonus : -vaccinePenalty;
        int totalScore = killScore + rescueScore + vaccineValue;

        view.UpdateScoreDisplay(
            killCount, killMultiplier, killScore,
            rescueCount, rescueMultiplier, rescueScore,
            isVaccineCleared, vaccineValue,
            totalScore
        );
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}