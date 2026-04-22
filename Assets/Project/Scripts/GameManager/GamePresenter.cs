using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePresenter : MonoBehaviour
{
    public static GamePresenter Instance { get; private set; }

    [SerializeField] private GameData gameData;
    [SerializeField] private TimerView timerView;

    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private NextLevelView nextLevelView;

    private GameModel model;
    private bool _isGameEnded = false;

    private void Awake()
    {
        SessionData.ResetData();

        if (Instance == null) Instance = this;
        model = new GameModel(gameData);

        model.OnTimeChanged += (time) =>
        {
            timerView.UpdateTimerDisplay(time);
        };

        model.OnTimeUp += () =>
        {
            timerView.ShowTimeUpMessage();
            TriggerGameOver();
        };

        timerView.UpdateTimerDisplay(model.CurrentTime);
    }

    private void Start()
    {
        UIManager.Instance.ShowMissionStartMessage();
        model.StartTimer();
    }

    private void Update()
    {
        model.Tick(Time.deltaTime);
    }

    public void TriggerGameOver()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;

        model.StopTimer();

        ShowGameOverUIAsync().Forget();
    }

    public void TriggerGameClear()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;

        model.StopTimer();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowGameClearUIAsync().Forget();
    }

    private async UniTaskVoid TransitionToResultAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(gameData.transitionWaitTime));
        SceneManager.LoadScene(gameData.resultSceneName);
    }

    private async UniTaskVoid ShowGameClearUIAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (nextLevelView != null)
        {
            nextLevelView.PlayComingSoonSequence().Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(1.0));

            timerView.Hide();
        }
        TransitionToResultAsync().Forget();
    }

    private async UniTaskVoid ShowGameOverUIAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverView != null)
        {
            gameOverView.PlayGameOverSequence().Forget();
        }

        timerView.Hide();

        TransitionToResultAsync().Forget();
    }
}
