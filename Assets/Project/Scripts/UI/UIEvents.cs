using System;

/// <summary>
/// UIに関するすべてのイベントを管理する静的クラス
/// 他のスクリプトはここを通じてテキストを呼び出す
/// </summary>
public static class UIEvents
{
    // === インタラクト関連 ===

    /// <summary>
    /// インタラクトの説明テキストを表示するイベント
    /// </summary>
    public static Action<string> OnShowInteractPrompt;

    /// <summary>
    /// インタラクトの案内テキストを隠すイベント
    /// </summary>
    public static Action OnHideInteractPrompt;

    // === システムメッセージ関連 ===

    /// <summary>
    /// カットシーンの開始・終了を通知するイベント
    /// </summary>
    public static Action<bool> OnCutsceneStateChanged;

    /// <summary>
    /// ミッション開始のメッセージを表示するイベント
    /// </summary>
    public static Action OnShowMissionStartMessage;

    /// <summary>
    /// 画面中央にシステムメッセージを表示するイベント
    /// 引数1: 表示するテキスト, 引数2: 表示し続ける秒数
    /// </summary>
    public static Action<string, float> OnShowSystemMessage;

    /// <summary>
    /// NPCが死亡した時の専用メッセージを表示するイベント
    /// </summary>
    public static Action OnShowNpcDeathMessage;

    /// <summary>
    /// ポーズしているかどうかを保持するイベント
    /// </summary>
    public static Action<bool> OnPauseStateChanged;

    /// <summary>
    /// 脱出テキストを表示・非表示をするイベント
    /// </summary>
    public static Action OnShowEscapeConfirm;
    public static Action OnHideEscapeConfirm;

    /// <summary>
    /// 階層が変わった（ロード完了後）ことを通知するイベント
    /// 引数: 次の階層の番号
    /// </summary>
    public static Action<int> OnFloorChanged;

    /// <summary>
    /// ロード画面が消えて、新しい階層が見えるようになったことを通知するイベント
    /// </summary>
    public static Action OnFloorTransitionVisible;

    /// <summary>
    /// ワクチンが回収されたことを通知するイベント
    /// </summary>
    public static Action<VaccineType> OnVaccineCollected;
}
