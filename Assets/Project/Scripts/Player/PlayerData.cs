using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float moveSpeed = 10f; // 移動速度
    public float rotationSensitivity = 0.1f; // 視点感度
    public float minPitch = -45f; // 視点上限
    public float maxPitch = 45f;  // 視点下限
}