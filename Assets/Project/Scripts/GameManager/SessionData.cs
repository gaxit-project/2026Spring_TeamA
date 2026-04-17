public class SessionData
{
    public static int KillCount { get; private set; } = 0;

    public static void AddKill()
    {
        KillCount++;
    }

    public static void ResetData()
    {
        KillCount = 0;
    }
}
