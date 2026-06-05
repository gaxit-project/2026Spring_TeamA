using UnityEngine;

public class ElectricPanel : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private int maxHP = 1;
    [SerializeField] private int dischargeDamage = 0; // 放電のダメージ量
    [SerializeField] private float dischargeRadius = 5.0f; // 放電の半径

    [Header("Effects")]
    [SerializeField] private GameObject dischargeEffect;
    [SerializeField] private AudioClip dischargeSound;
    [SerializeField] private float soundVolume = 1.0f;

    private int _currentHP;
    private bool _hasDischarged = false;

    private void Awake()
    {
        _currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (_hasDischarged) return;

        _currentHP -= amount;

        if(_currentHP <= 0)
        {
            Discharge();
        }
    }

    private void Discharge()
    {
        _hasDischarged = true;

        if (dischargeEffect != null)
        {
            Instantiate(dischargeEffect, transform.position, Quaternion.identity);
        }

        if (dischargeSound != null)
        {
            //要修正
            //soundVolume = SoundManager.Instance.GetSEVolume();
            SoundManager.Instance.PlaySound(dischargeSound);
            Debug.Log("play discharge SE");
        }

        // 周囲へのダメージ判定 
        // 放電地点を中心に、半径 electricRadius 内のコライダーをすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, dischargeRadius);

        foreach (var hitCollider in hitColliders)
        {
            // 自分自身（パネル）は無視するように
            if (hitCollider.gameObject == gameObject) continue;
            // 当たったオブジェクトまたはその親から IDamageable を探す
            var damageable = hitCollider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                // ダメージを与える
                damageable.TakeDamage(dischargeDamage);
                Debug.Log($"Electric hit: {hitCollider.name} Damage: {dischargeDamage}");
            }

            // 当たったオブジェクトがゾンビなら感電させる
            if(hitCollider.GetComponentInParent<EnemyPresenter>() is EnemyPresenter enemy)
            {
                enemy.Shocked();
            }
        }

        Debug.Log("discharge Panel");

        //オブジェクトは残す
        //Destroy(gameObject);
        gameObject.GetComponent<Renderer>().material.color = Color.gray;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dischargeRadius);
    }
}
