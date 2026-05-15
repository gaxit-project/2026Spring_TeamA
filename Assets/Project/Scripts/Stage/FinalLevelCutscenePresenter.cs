using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 最終階層でのカメラ演出の進行を制御するPresenter
/// </summary>
public class FinalLevelCutscenePresenter : MonoBehaviour
{
    [SerializeField, Tooltip("演出用のView")] 
    private FinalLevelCutsceneView _view;

    [SerializeField, Tooltip("演出を再生する階層")]
    private int _targetFloorToPlay = 3;

    private bool _playOnlyOnce = true;
    private bool _hasPlayed = false;

    private UniTaskCompletionSource _transitionVisibleTcs;

    private void OnEnable()
    {
        UIEvents.OnFloorChanged += OnFloorChanged;
        UIEvents.OnFloorTransitionVisible += OnTransitionVisible;
    }

    private void OnDestroy()
    {
        UIEvents.OnFloorChanged -= OnFloorChanged;
        UIEvents.OnFloorTransitionVisible -= OnTransitionVisible;
    }

    private void OnTransitionVisible()
    {
        _transitionVisibleTcs?.TrySetResult();
    }

    private void OnFloorChanged(int floor)
    {
        if (_playOnlyOnce && _hasPlayed) return;

        if (floor == _targetFloorToPlay)
        {
            _hasPlayed = true;
            PlayCutsceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    /// <summary>
    /// カットシーンを非同期で再生する
    /// </summary>
    public async UniTask PlayCutsceneAsync(CancellationToken token)
    {
        if (_view == null) return;

        // 【準備】画面が暗いうちにUIを隠し、カメラをセットしておく
        _view.HideUI();
        _view.ShowVaccineCamera(0);
        _view.HideText(); // テキストはまだ出さない

        // ロード画面が明けるのを待機する
        _transitionVisibleTcs = new UniTaskCompletionSource();
        await _transitionVisibleTcs.Task;

        // 【演出開始】画面が見えた瞬間に時間を止め、1つ目のテキストを表示する
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _view.ShowVaccineCamera(0);

        try
        {
            // 【再生】ここからカウントダウン開始
            for (int i = 0; i < _view.VaccineCameraCount; i++)
            {
                // 1つ目は既に表示中（テキストセット済み）なので、2つ目以降から切り替える
                if (i > 0) _view.ShowVaccineCamera(i);
                await UniTask.Delay(TimeSpan.FromSeconds(_view.CameraDuration), ignoreTimeScale: true, cancellationToken: token);
            }

            // 出口カメラを表示
            _view.ShowExitCamera();
            await UniTask.Delay(TimeSpan.FromSeconds(_view.CameraDuration), ignoreTimeScale: true, cancellationToken: token);
        }
        finally
        {
            // カットシーン終了後のクリーンアップ処理（キャンセル時も確実に復旧する）
            _view.HideAllCameras();
            _view.HideText();
            _view.ShowUI();
            Time.timeScale = originalTimeScale;
        }
    }
}
