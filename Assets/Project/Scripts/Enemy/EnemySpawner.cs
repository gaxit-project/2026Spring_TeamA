using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPointConfig
    {
        public Transform point; // 生成する場所
        public int count = 1;   // 何体生成するか
    }

    [SerializeField] private GameObject enemyPrefab; // ゾンビのプレハブ
    [SerializeField] private List<SpawnPointConfig> spawnPoints; // 生成ポイントのリスト

    private void Start()
    {
        SpawnEnemies();
    }

    /// <summary>
    /// 設定されたポイントに敵を生成する
    /// </summary>
    public void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab がセットされていません！");
            return;
        }

        foreach (var config in spawnPoints)
        {
            if (config.point == null) continue;

            for (int i = 0; i < config.count; i++)
            {
                // 指定された位置と回転で生成
                GameObject enemy = Instantiate(enemyPrefab, config.point.position, config.point.rotation);

                // ゾンビの名前を少し分かりやすく変更（任意）
                enemy.name = $"{enemyPrefab.name}_{config.point.name}_{i}";
            }
        }
    }
}
