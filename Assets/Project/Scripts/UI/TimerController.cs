using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private int timeLimit = 600;
    private float _remainingTime;
    private TextMeshProUGUI _timerUI;
    private bool _isRunning = true;

    void Start()
    {
        _remainingTime = timeLimit;
        
        // TextMeshProUGUIを取得
        if (GetComponent<TextMeshProUGUI>() is TextMeshProUGUI timer) _timerUI = timer;
        else Debug.LogError("TimerController:TextMeshProUGUIが見つかりません！");
    }

    void Update()
    {
        // タイマーが動作中のみ時間を減らす
        if (_isRunning)
        {
            _remainingTime -= Time.deltaTime;
            DisplayTime(_remainingTime);
        }
    }

    private void DisplayTime(float time)
    {
        // 残り時間が0になったらタイマーストップ
        if(time < 0f)
        {
            _isRunning = false;
            _timerUI.color = Color.red;
        }

        time += 1;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        // 分:秒 の形式で表示
        _timerUI.text = $"{minutes:00}:{seconds:00}";
    }
}
