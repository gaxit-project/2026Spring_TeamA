using UnityEngine;

public class EnemyModel 
{
    private readonly EnemyData _enemyData;

    public int CullentHP { get; set; }

    public EnemyModel(EnemyData enemyData)
    {
        _enemyData = enemyData;
        CullentHP = enemyData.enemyHP; 
    }

    public void TakeDamage(int amount)
    {
        CullentHP -= amount;
        Debug.Log($"EnemyHP:{CullentHP}");
    }
}
