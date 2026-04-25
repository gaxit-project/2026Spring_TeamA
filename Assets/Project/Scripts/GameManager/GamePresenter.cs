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
            PlayerPresenter.Instance.DisableInput(true);
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

        // プレイヤーの操作をブロック
        if (PlayerPresenter.Instance != null)
        {
            PlayerPresenter.Instance.DisableInput(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // プレイ中のUIを非表示にする
        if (timerView != null) timerView.Hide();
        if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();

        ShowGameClearUIAsync().Forget();
    }

    private async UniTaskVoid TransitionToResultAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(gameData.transitionWaitTime));
        SceneManager.LoadScene(gameData.resultSceneName);
    }

    private async UniTaskVoid ShowGameClearUIAsync()
    {
        // 少し間を開ける
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        
        if (nextLevelView != null)
        {
            // ホワイトアウトとテキスト表示終わるまで待機
            await nextLevelView.PlaySequence(true);
        }

        // リザルトへ遷移
        TransitionToResultAsync().Forget();
    }

    public void TriggerNextLevel()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;

        model.StopTimer();

        // プレイヤーの操作をブロック
        if (PlayerPresenter.Instance != null)
        {
            PlayerPresenter.Instance.DisableInput(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // プレイ中のUIを非表示にする
        if (timerView != null) timerView.Hide();
        if (UIManager.Instance != null) UIManager.Instance.HideInteractPrompt();

        ShowNextLevelUIAsync().Forget();
    }

    private async UniTaskVoid TransitionToNextLevelAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(gameData.transitionWaitTime));
        
        // GameDataに設定された次のシーンをロード。空なら現在のシーンを再読み込み
        if (!string.IsNullOrEmpty(gameData.nextLevelSceneName))
        {
            SceneManager.LoadScene(gameData.nextLevelSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private async UniTaskVoid ShowNextLevelUIAsync()
    {
        // 少し間を開ける
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        
        if (nextLevelView != null)
        {
            // 暗転とLoading...テキスト表示が終わるまで待機 
            await nextLevelView.PlaySequence(false);
        }

        // 次の階層へ遷移
        TransitionToNextLevelAsync().Forget();
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