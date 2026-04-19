using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー自身に当たった場合は無視する（レイヤー設定でも可）
        if (other.CompareTag("Player")) return;
        // 何かに当たったら、そこで弾丸を消す
        Destroy(gameObject);

    }
}
