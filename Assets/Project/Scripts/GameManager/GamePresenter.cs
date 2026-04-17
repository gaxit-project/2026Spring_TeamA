using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    [SerializeField] private TimerView timerView;

    private GameModel model;

    private void Awake()
    {
        model = new GameModel(gameData);

        model.OnTimeChanged += (time) =>
        {
            timerView.UpdateTimerDisplay(time);
        };

        model.OnTimeUp += () =>
        {
            timerView.ShowTimeUpMessage();
            HandleTimeUp();
        };

        timerView.UpdateTimerDisplay(model.CurrentTime);
    }

    private void Start()
    {
        model.StartTimer();
    }

    private void Update()
    {
        model.Tick(Time.deltaTime);
    }

    private void HandleTimeUp()
    {
        Debug.Log("Time's Up! Game Over!");
    }
}
