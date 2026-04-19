using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float spawnRadius = 2.0f; // どのくらいの範囲に散らすか


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
                // 指定された場所から半径spawnRadius内のランダムな位置を計算
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0; // 高さは変えない
                Vector3 spawnPos = config.point.position + randomOffset;
                // NavMesh上の有効な地点かどうかをチェック
                // 壁の中や空中を避けて、最も近い地面を探す
                if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
                {
                    spawnPos = hit.position;
                }
                // 3. 生成
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, config.point.rotation);
                enemy.name = $"{enemyPrefab.name}_{config.point.name}_{i}";
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        foreach (var config in spawnPoints)
        {
            if (config.point != null)
            {
                Gizmos.DrawSphere(config.point.position, spawnRadius);
            }
        }
    }
}
