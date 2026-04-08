using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private int _timeLimit = 600;
    private float _remainingTime;
    private TextMeshProUGUI _timerUI;

    void Start()
    {
        _remainingTime = _timeLimit;
        if(GetComponent<TextMeshProUGUI>() is TextMeshProUGUI timer) _timerUI = timer; 
    }

    void Update()
    {
        
    }
}
