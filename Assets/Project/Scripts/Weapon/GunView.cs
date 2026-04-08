using UnityEngine;

public class GunView : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // 銃弾のプレハブ
    public Transform muzzlePoint; // 弾が出る位置
    [SerializeField] private ParticleSystem muzzleFlash; // 銃のフラッシュ

    [SerializeField] private AudioSource audioSource;

    public void PlayShotSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // PlayOneShot: 連射しても音が途切れずに重なって聞こえる
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySimpleSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource != null) audioSource.Stop();
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
