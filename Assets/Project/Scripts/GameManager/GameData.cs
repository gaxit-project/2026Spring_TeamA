using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameData : ScriptableObject
{
    [Header("Time Settings")]
    public float gameTimeSeconds = 60f; // 制限時間（秒）
    public string resultSceneName = "Result";
    public float transitionWaitTime = 3.0f;
}
