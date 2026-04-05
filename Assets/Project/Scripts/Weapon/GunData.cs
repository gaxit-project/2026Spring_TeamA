using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public int damage; // 攻撃力
    public float speed; // スピード
    public int maxAmmo; // 装填弾数
    public float fireRate; // 連射速度（秒）
    public float reloadTime; // リロード時間（秒）
}
