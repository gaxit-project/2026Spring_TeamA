using UnityEngine;
using TMPro;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    /// <summary>
    /// 残り時間を1:23のようにする
    /// </summary>
    public void UpdateTimerDisplay(float currentTime)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // {0:00} は2桁のゼロ埋め（例：5秒なら 05 と表示）
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowTimeUpMessage()
    {
        if (timerText != null)
        {
            timerText.text = "00:00";
            timerText.color = Color.red;
        }
    }

    public void Show()
    {
        // 再表示する用
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
