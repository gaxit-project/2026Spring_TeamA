using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Visual Settings")]
    public AnimatorOverrideController controller;   // アニメーション差し替え用

    public int enemyHP = 100;
    public float moveSpeed = 3.0f;
    public int enemyAttackPower = 30;

    public float fieldOfView = 140f; // 視野角度
    public float detectionRange = 10f;  // 検出範囲

    [Header("Distance Setting")]
    public int attackDistance = 1;
    public float windowKnockDistance = 4.0f;
    public float attackRangeOffset = 0;
}
