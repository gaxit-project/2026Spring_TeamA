using UnityEngine;

public class SoundDetectionView : MonoBehaviour
{
    public static SoundDetectionView Instance { get; private set; }

    public System.Action<Collider> HitEnemy;
    public LayerMask enemyLayer;
    public float radius = 15f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SoundSource(Vector3 center)
    {
        Collider[] enemies = Physics.OverlapSphere(center, radius, enemyLayer);

        if (enemies.Length > 0)
        {
            foreach (Collider enemy in enemies)
            {
                HitEnemy?.Invoke(enemy);   // 通知
            }
        }
    }

    /// <summary>
    /// 音が届く範囲を可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
