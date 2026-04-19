using UnityEngine;

public class PlayerModel
{
    // 設定データ（ScriptableObject）への参照
    private readonly PlayerData _data;

    public int CurrentHP { get; private set; }

    public System.Action<int> OnHpChanged;
    private float _lastDamageTime;

    // 現在の入力値を保持するプロパティ（Presenterから更新される）
    public Vector2 MoveInput { get; set; }
    public float CurrentPan { get; set; }
    public float currentPitch { get; set; }
    public bool IsDashing { get; set; }

    // コンストラクタ：初期化時にPresenterからデータを入れてもらう
    public PlayerModel(PlayerData data)
    {
        _data = data;
        CurrentHP = data.hp;
    }

    public Vector3 CalcMove(float deltaTime)
    {
        float speed = IsDashing ? _data.moveDashSpeed : _data.moveSpeed;
        return new Vector3(MoveInput.x, 0, MoveInput.y) * speed * deltaTime;
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
}