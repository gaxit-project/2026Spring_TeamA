using UnityEngine;

public class EnemyModel
{
    private readonly EnemyData _enemyData;

    public int CurrentHP { get; set; }

    public EnemyModel(EnemyData enemyData)
    {
        _enemyData = enemyData;
        CurrentHP = enemyData.enemyHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        Debug.Log($"EnemyHP:{CurrentHP}");
    }
}
