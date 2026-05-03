using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "ScriptableObjects/BossData")]
public class BossData : ScriptableObject
{
    [Header("基本ステータス")]
    public int maxHp = 100;
    public float moveSpeed = 3.5f;

    [Header("攻撃設定")]
    public int attackDamage = 50;
    public float attackRange = 2.5f;
    public float attackInterval = 3.0f;
    public float stoppingDistance = 2.0f;

    [Header("ダウン（ひざまずき）設定")]
    public float stunDuration = 8.0f;
    public int stunDamageThreshold = 5;
}
