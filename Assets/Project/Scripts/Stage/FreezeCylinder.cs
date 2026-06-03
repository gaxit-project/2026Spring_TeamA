using UnityEngine;

public class FreezeCylinder : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private int maxHP = 1;
    [SerializeField] private int freezeDamage = 10; // 凍結のダメージ量
    [SerializeField] private float freezeRadius = 5.0f; // 凍結の半径

    [Header("Effects")]
    [SerializeField] private GameObject freezeEffect;
    [SerializeField] private AudioClip freezeSound;
    [SerializeField] private float soundVolume = 1.0f;

    private int _currentHP;
    private bool _hasfreezed = false;

    private void Awake()
    {
        _currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (_hasfreezed) return;

        _currentHP -= amount;

        if(_currentHP <= 0)
        {
            Freeze();
        }
    }

    private void Freeze()
    {
        _hasfreezed = true;

        if (freezeEffect != null)
        {
            Instantiate(freezeEffect, transform.position, Quaternion.identity);
        }

        if (freezeSound != null)
        {
            //要修正
            //soundVolume = SoundManager.Instance.GetSEVolume();
            SoundManager.Instance.PlaySound(freezeSound);
            Debug.Log("play freeze SE");
        }

        // 周囲へのダメージ判定 
        // 凍結地点を中心に、半径 freezeRadius 内のコライダーをすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, freezeRadius);

        foreach (var hitCollider in hitColliders)
        {
            // 自分自身（ボンベ）は無視するように
            if (hitCollider.gameObject == gameObject) continue;
            // 当たったオブジェクトまたはその親から IDamageable を探す
            var damageable = hitCollider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                // ダメージを与える
                damageable.TakeDamage(freezeDamage);
                Debug.Log($"Freeze hit: {hitCollider.name} Damage: {freezeDamage}");
            }

            // 当たったオブジェクトがゾンビなら凍結させる
            if (hitCollider.GetComponent<EnemyView>() is EnemyView enemy)
            {
                enemy.Frozen();
            }
        }

        Debug.Log("freeze Cylinder");

        //オブジェクトは残す
        //Destroy(gameObject);
        gameObject.GetComponent<Renderer>().material.color = Color.gray;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, freezeRadius);
    }
}
