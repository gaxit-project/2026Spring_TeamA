using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 1;
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
            // TODO 無理やり音量アップを修正する
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
            Debug.Log("play explosion SE");
        }

        Debug.Log("explode Barrel");
        Destroy(gameObject);
    }
}
