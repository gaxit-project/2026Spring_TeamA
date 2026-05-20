using UnityEngine;

public class GunView : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // 銃弾のプレハブ
    public Transform muzzlePoint; // 弾が出る位置
    [SerializeField] private ParticleSystem muzzleFlash; // 銃のフラッシュ

    [SerializeField] private AudioSource audioSource;

    public void PlayShotSound(AudioClip clip)
    {
        if(clip != null && audioSource != null)
        {
            // PlayOneShot: 連射しても音が途切れずに重なって聞こえる
            audioSource.volume = SoundManager.Instance.GetSEVolume() * 0.5f;
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySimpleSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.volume = SoundManager.Instance.GetSEVolume();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource != null) audioSource.Stop();
    }

    /// <summary>
    /// 着弾した時の処理（ガラスを割ったり、敵にダメージを与えたり）
    /// </summary>
    public virtual void ProcessHit(RaycastHit hit, int damage)
    {
        var bodyPart = hit.collider.GetComponent<EnemyBodyPart>();
        if (bodyPart != null)
        {
            bodyPart.NotifyHit(damage);
            return;
        }
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null) damageable.TakeDamage(damage);
        var glass = hit.collider.GetComponent<BulletproofGlass>();
        if (glass != null) glass.AddCrack(hit.point, hit.normal);
    }

    public void LaunchBullet(Vector3 targetDirection, float speed)
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(targetDirection));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = targetDirection * speed;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }
}
