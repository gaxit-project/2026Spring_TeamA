using UnityEngine;

public class SoundDetectionView : MonoBehaviour
{
    public System.Action<Collider> HitEnemy;

    public LayerMask enemyLayer;

    public void SoundSource(Vector3 center, float radius)
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
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
