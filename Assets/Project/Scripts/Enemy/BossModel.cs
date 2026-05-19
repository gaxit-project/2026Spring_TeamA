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

    private int _accumulatedDamage = 0;

    /// <summary>
    /// ボスのデータモデルを初期化
    /// </summary>
    public BossModel(BossData data)
    {
        Data = data;
        CurrentHp = data.maxHp;
        CurrentState = BossState.Chase;
        _accumulatedDamage = 0;
    }

    /// <summary>
    /// ダメージを処理し、累積ダメージがしきい値に達したらスタン状態にする
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (CurrentState == BossState.Dead) return;

        CurrentHp -= damage;
        OnHpChanged?.Invoke(CurrentHp);

        if (CurrentHp <= 0)
        {
            SetState(BossState.Dead);
        }
        else if (CurrentState != BossState.Stunned)
        {
            _accumulatedDamage += damage;
            if (_accumulatedDamage >= Data.stunDamageThreshold)
            {
                _accumulatedDamage = 0;
                SetState(BossState.Stunned);
            }
        }
    }

    public void SetState(BossState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}
