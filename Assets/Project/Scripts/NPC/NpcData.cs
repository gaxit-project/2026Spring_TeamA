using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableObjects/NPCData")]
public class NPCData : ScriptableObject
{
    [Header("確率")]
    [Range(0f, 1f), Tooltip("助かる（素直に言うことを聞く）確率")]
    public float rescueSuccessProbability = 0.5f;

    [Header("安心パターン（成功時）")]
    [Tooltip("喜ぶアニメーションが終わって歩き出すまでの待機秒数")]
    public float timeToStartWalking = 2.0f;
    [Tooltip("歩き去る時の速度")]
    public float relievedWalkSpeed = 2.0f;
    [Tooltip("歩き去る距離（プレイヤーの後方何メートルか）")]
    public float relievedWalkDistance = 5.0f;
    [Tooltip("歩き始めてから消滅するまでの秒数")]
    public float destroyDelayAfterWalk = 4.0f;

    [Header("パニックパターン（逃げる時）")]
    [Tooltip("逃げ走る時の速度")]
    public float panicRunSpeed = 6.0f;
    [Tooltip("逃げ走る距離（プレイヤーの逆方向何メートルか）")]
    public float panicRunDistance = 15.0f;
    [Tooltip("逃げ始めてから消滅するまでの秒数")]
    public float destroyDelayAfterPanic = 4.0f;
}