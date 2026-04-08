using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject gunPrefab;
    public AnimatorOverrideController animatorOverride;　// 銃ごとのアニメーション
    public bool isFullAuto; // チェックを入れるとフルオート、外すとセミオート

    [Header("Gun Parameters")]
    public int damage; // 攻撃力
    public float speed; // スピード
    public int maxAmmo; // 装填弾数
    public int initAmmo; // ゲーム開始時に所持している弾数
    public float fireRate; // 連射速度（秒）
    public float reloadTime; // リロード時間（秒）

    [Header("Audio Settings")]
    public AudioClip fireSound; // 銃声
    public AudioClip reloadSound; // リロード音
    public AudioClip emptySound; // 弾切れ時のカチッという音
    public AudioClip drawSound; // 武器を取り出した時の音
}
