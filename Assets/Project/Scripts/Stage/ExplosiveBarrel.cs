using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private int maxHP = 1;
    [SerializeField] private int explosionDamage = 100; // 爆発のダメージ量
    [SerializeField] private float explosionRadius = 5.0f; // 爆発の半径

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float soundVolume = 1.0f;

    private int _currentHP;
    private bool _hasExploded = false;

    private void Awake()
    {
        _currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (_hasExploded) return;

        _currentHP -= amount;

        if(_currentHP <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            soundVolume = SoundManager.Instance.GetSEVolume();
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
            Debug.Log("play explosion SE");
        }

        // 周囲へのダメージ判定 
        // 爆発地点を中心に、半径 explosionRadius 内のコライダーをすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // 自分自身（ドラム缶）は無視するように、もし他のドラム缶も爆発させたいならそのまま
            if (hitCollider.gameObject == gameObject) continue;
            // 当たったオブジェクトまたはその親から IDamageable を探す
            var damageable = hitCollider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                // ダメージを与える
                damageable.TakeDamage(explosionDamage);
                Debug.Log($"Explosion hit: {hitCollider.name} Damage: {explosionDamage}");
            }
        }

        Debug.Log("explode Barrel");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
