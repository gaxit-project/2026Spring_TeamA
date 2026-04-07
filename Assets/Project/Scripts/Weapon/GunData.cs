using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject gunPrefab;
    public AnimatorOverrideController animatorOverride;　// 銃ごとのアニメーション
    public bool isFullAuto; // チェックを入れるとフルオート、外すとセミオート
    public int damage; // 攻撃力
    public float speed; // スピード
    public int maxAmmo; // 装填弾数
    public int initAmmo; // ゲーム開始時に所持している弾数
    public float fireRate; // 連射速度（秒）
    public float reloadTime; // リロード時間（秒）
}
