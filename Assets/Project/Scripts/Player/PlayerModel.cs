using UnityEngine;

public class PlayerModel
{
    // 設定データ（ScriptableObject）への参照
    private readonly PlayerData _data;

    public int CurrentHP { get; private set; }
    private float _lastDamageTime;

    // 現在の入力値を保持するプロパティ（Presenterから更新される）
    public Vector2 MoveInput { get; set; }
    public float CurrentPan { get; set; }
    public float currentPitch { get; set; }

    // コンストラクタ：初期化時にPresenterからデータを入れてもらう
    public PlayerModel(PlayerData data)
    {
        _data = data;
        CurrentHP = data.hp;
    }

    public Vector3 CalcMove(float deltaTime)
    {
        return new Vector3(MoveInput.x, 0, MoveInput.y) * _data.moveSpeed * deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (Time.time < _lastDamageTime + _data.damageInterval) return;

        CurrentHP -= amount;
        _lastDamageTime = Time.time;
        Debug.Log($"Player HP: {CurrentHP}");

        if (CurrentHP <= 0)
        {
            // 死亡処理
        }
    }
}