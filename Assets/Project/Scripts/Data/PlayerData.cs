using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int hp = 100; // プレイヤーの体力
    public float moveSpeed = 10f; // 移動速度
    public float moveDashSpeed = 20f; // ダッシュ
    public float rotationSensitivity = 0.1f; // 視点感度
    public float minPitch = -45f; // 視点上限
    public float maxPitch = 45f;  // 視点下限
    public string zombieTag = "Zombie"; // ダメージ処理で使うゾンビのタグ名
    public float damageInterval = 1.0f; // ダメージを受ける間隔（秒）
}