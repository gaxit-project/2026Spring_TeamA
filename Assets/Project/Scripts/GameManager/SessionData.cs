public class SessionData
{
    public static int KillCount { get; private set; } = 0;

    public static int RescueCount { get; private set; } = 0;

    public static void AddKill()
    {
        KillCount++;
    }

    public static void AddRescue()
    {
        RescueCount++;
    }

    public static void ResetData()
    {
        KillCount = 0;
        RescueCount = 0;
    }
}
