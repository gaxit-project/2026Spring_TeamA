using UnityEngine;

public class GunView : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // 銃弾のプレハブ
    public Transform muzzlePoint; // 弾が出る位置
    [SerializeField] private ParticleSystem muzzleFlash;

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
