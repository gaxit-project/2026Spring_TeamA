using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int enemyHP = 100;
    public float moveSpeed = 3.0f;
    public int enemyAttackPower = 30;
    public int attackDistance = 1;
}
