using System;
using UnityEngine.InputSystem.LowLevel;

public class BossModel
{
    public enum BossState { Chase, Attack, Stunned, Dead }
    
    public int CurrentHp { get; private set; }
    public BossState CurrentState { get; private set; }
    public BossData Data { get; private set; }

    public Action<BossState> OnStateChanged;
    public Action<int> OnHpChanged;

    /// <summary>
    /// ボスのデータモデルを初期化
    /// </summary>
    public BossModel(BossData data)
    {
        Data = data;
        CurrentHp = data.maxHp;
        CurrentState = BossState.Chase;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentState == BossState.Dead) return;

        CurrentHp -= damage;
        OnHpChanged?.Invoke(CurrentHp);

        if (CurrentHp <= 0)
        {
            SetState(BossState.Dead);
        }
        else if (damage >= Data.stunDamageThreshold && CurrentState != BossState.Stunned)
        {
            SetState(BossState.Stunned);
        }
    }

    public void SetState(BossState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}
