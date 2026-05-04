public class SessionData
{
    public static int KillCount { get; private set; } = 0;

    public static int RescueCount { get; private set; } = 0;

    public static bool IsVaccineCleared { get; private set; } = false;

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
    /// データのリセット
    /// </summary>
    public static void ResetData()
    {
        KillCount = 0;
        RescueCount = 0;
        IsVaccineCleared = false;
    }
}
