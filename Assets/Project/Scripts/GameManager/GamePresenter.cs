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
            if (PlayerPresenter.Instance != null)
            {
                PlayerPresenter.Instance.DisableInput(true);
            }
            TriggerGameOver();
        };

        timerView.UpdateTimerDisplay(model.CurrentTime);
    }

    private void Start()
    {
        UIEvents.OnShowMissionStartMessage?.Invoke();

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
        UIEvents.OnHideInteractPrompt?.Invoke();

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
        UIEvents.OnHideInteractPrompt?.Invoke();

        ShowNextLevelUIAsync().Forget();
    }

    public void TriggerWarp(Transform destination)
    {
        if (_isGameEnded) return;

        WarpSequenceAsync(destination).Forget();
    }

    private async UniTaskVoid WarpSequenceAsync(Transform destination)
    {
        // ワープ中に動けないように、プレイヤーの入力を止める
        if (PlayerPresenter.Instance != null)
        {
            PlayerPresenter.Instance.SetInputBlocked(true);
            PlayerPresenter.Instance.PlayerView.ForceIdle();
        }

        // フェードインを行う
        if (nextLevelView != null)
        {
            nextLevelView.gameObject.SetActive(true);
            await nextLevelView.PlaySequence(false);
        }

        // 裏でプレイヤーの座標を移動
        if (destination != null && PlayerPresenter.Instance != null)
        {
            PlayerPresenter.Instance.transform.position = destination.position;
            PlayerPresenter.Instance.transform.rotation = destination.rotation;
        }

        // 余韻
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));

        // ロード画面をフェードアウト
        if (nextLevelView != null)
        {
            await nextLevelView.FadeOutAsync();
        }


        // ロード完了後にプレイヤーの階層を+1する
        if (PlayerPresenter.Instance != null)
        {
            int nextFloor = PlayerPresenter.Instance.CurrentFloor + 1;
            PlayerPresenter.Instance.SetFloor(nextFloor);

            // プレイヤーの操作を再開
            PlayerPresenter.Instance.SetInputBlocked(false);
        }

        // プレイヤーの操作を再開
        if (PlayerPresenter.Instance != null)
            PlayerPresenter.Instance.SetInputBlocked(false);
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