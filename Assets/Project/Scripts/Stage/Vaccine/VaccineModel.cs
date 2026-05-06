using System;


/// <summary>
/// ワクチンの収集状態を管理するモデル
/// </summary>
public class VaccineModel
{
    private const int MaxVaccines = 3;
    public int CollectedCount { get; private set; } = 0;
    public bool IsAllCollected => CollectedCount >= MaxVaccines;

    public event Action<int> OnVaccineCollected;
    public event Action OnAllVaccinesCollected;

    /// <summary>
    /// ワクチンを1つ収集する
    /// </summary>
    public void Collect()
    {
        if (IsAllCollected) return;
        CollectedCount++;
        OnVaccineCollected?.Invoke(CollectedCount);
        if (IsAllCollected)
        {
            OnAllVaccinesCollected?.Invoke();
        }
    }
}
