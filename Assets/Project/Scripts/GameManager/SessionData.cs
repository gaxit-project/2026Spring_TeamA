public class SessionData
{
    public static int KillCount { get; private set; } = 0;

    public static int RescueCount { get; private set; } = 0;

    public static bool IsVaccineCleared { get; private set; } = false;

    public static bool IsGameClear { get; private set; } = false;

    public static float RemainingTime { get; private set; } = 0f;

    /// <summary>
    /// 倒したゾンビのカウントを増やす
    /// </summary>
    public static void AddKill()
    {
        KillCount++;
    }

    /// <summary>
    /// 助けたNPCのカウントを増やす
    /// </summary>
    public static void AddRescue()
    {
        RescueCount++;
    }

    /// <summary>
    /// ワクチンクリアフラグの設定
    /// </summary>
    public static void SetVaccineClear(bool isCleared)
    {
        IsVaccineCleared = isCleared;
    }

    /// <summary>
    /// ゲームクリアフラグの設定
    /// </summary>
    public static void SetGameClear(bool isClear)
    {
        IsGameClear = isClear;
    }

    /// <summary>
    /// 残り時間の設定
    /// </summary>
    public static void SetRemainingTime(float time)
    {
        RemainingTime = time;
    }

    /// <summary>
    /// データのリセット
    /// </summary>
    public static void ResetData()
    {
        KillCount = 0;
        RescueCount = 0;
        IsVaccineCleared = false;
        IsGameClear = false;
        RemainingTime = 0f;
    }
}
