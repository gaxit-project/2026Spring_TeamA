using UnityEngine;

public class PlayerModel
{
    // 設定データ（ScriptableObject）への参照
    private readonly PlayerData _data;

    public int CurrentHP { get; private set; }

    public event System.Action<int> OnHpChanged;

    private float _lastDamageTime;

    // 現在の入力値を保持するプロパティ（Presenterから更新される）
    public Vector2 MoveInput { get; set; }
    public float CurrentPan { get; set; }
    public float CurrentPitch { get; set; }

    public bool IsAiming { get; set; } = false;

    public int CurrentFloor { get; private set; } = 1;
    public event System.Action<int> OnFloorChanged;

    // コンストラクタ：初期化時にPresenterからデータを入れてもらう
    public PlayerModel(PlayerData data)
    {
        _data = data;
        CurrentHP = data.hp;
    }

    /// <summary>
    /// 移動速度（moveSpeed）を使用して移動量を計算する
    /// </summary>
    public Vector3 CalcMove(float deltaTime)
    {
        return new Vector3(MoveInput.x, 0, MoveInput.y) * _data.moveSpeed * deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHP <= 0) return;
        if (Time.time < _lastDamageTime + _data.damageInterval) return;

        CurrentHP -= amount;
        _lastDamageTime = Time.time;
        Debug.Log($"Player HP: {CurrentHP}");

        OnHpChanged?.Invoke(CurrentHP);
    }

    /// <summary>
    /// 階層を更新する
    /// </summary>
    public void SetFloor(int floor)
    {
        if (CurrentFloor == floor) return;

        CurrentFloor = floor;
        OnFloorChanged?.Invoke(CurrentFloor);
    }
}