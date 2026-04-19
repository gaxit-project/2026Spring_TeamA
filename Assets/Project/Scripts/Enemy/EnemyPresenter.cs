using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class EnemyPresenter : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyView view;    // 操作対象のview
    [SerializeField] private EnemyData enemyData;   // ScriptableObject
    [SerializeField] private GunData gunData;   // ScriptableObject
    [SerializeField] private List<EnemyBodyPart> bodyParts;  // 当たり判定リスト
    [SerializeField] private SoundDetectionView soundView;

    private EnemyModel model;

    private GameObject _target;     // 追跡対象
    private bool _isDead = false;   // 死亡判定

    // 子要素にあるものはエディタ上で事前に埋めて保存する
    private void OnValidate()
    {
        // Viewが未設定なら自身から取得
        if (view == null) view = GetComponent<EnemyView>();
        // 部位判定リストを子オブジェクトから自動取得
        if (bodyParts == null || bodyParts.Count == 0)
        {
            bodyParts = new List<EnemyBodyPart>(GetComponentsInChildren<EnemyBodyPart>(true));
        }
    }


    private void Awake()
    {
        // Modelに ScriptableObject を渡して初期化
        model = new EnemyModel(enemyData);

        // EnemyViewの衝突イベントを購読し,PlayerViewのへ反映させる
        view.OnContactStay += HandlePlayerContact;

        // EnemyViewでプレイヤーの位置を購読しEnemyViewへ反映させる
        view.OnFoundPlayer += HandleFoundPlayer;

        view.HitContact += OnHit; 

        if (soundView == null)
        {
            soundView = SoundDetectionView.Instance;
        }

        if (soundView != null)
        {
            soundView.HitEnemy += HandleSoundDetected;
        }
    }

    private void OnDestroy()
    {
        if(view != null)
        {
            view.OnContactStay -= HandlePlayerContact;
            view.OnFoundPlayer -= HandleFoundPlayer;
            view.HitContact -= OnHit;
        }

        if(soundView != null)
        {
            soundView.HitEnemy -= HandleSoundDetected;
        }
    }

    private void HandleSoundDetected(Collider col)
    {
        if (_isDead)
        {
            return;
        }
        if (col.gameObject == view.gameObject || col.transform.IsChildOf(transform))
        {
            view.SetHearing(true);
        }
    }

    private void HandlePlayerContact(Collider col)
    {
        if(_isDead)
        {
            return;
        }
        if(col.CompareTag("Player"))
        {
            var playerView = col.GetComponent<PlayerView>();
            playerView?.OnHitByEnemy?.Invoke(enemyData);
        }
    }

    private void HandleFoundPlayer(Vector3 pos)
    {
        if (_isDead)
        {
            return;
        }
        view.isTracking = true;
        view.MoveTo(pos);
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        // ModelにHPを計算させる
        model.TakeDamage(amount);

        view.Hit().Forget();

        // 死亡処理
        if (model.CurrentHP <= 0 && !_isDead)
        {
            _isDead = true;

            SessionData.AddKill();

            view.Die();
            HandleDeathAsync().Forget();
        }
    }

    /// <summary>
    /// 攻撃があたった部位とダメージ量の処理を行う
    /// </summary>
    private void OnHit(int damage, Collider hitCollider)
    {
        Debug.Log($"OnHit called. Damage: {damage}, HitCollider: {hitCollider.name}");

        var hitPart = bodyParts.Find(x => x.GetComponent<Collider>() == hitCollider);
        if (hitPart == null)
        {
            Debug.LogWarning($"BodyPart not found for {hitCollider.name}! list count: {bodyParts.Count}");
            return;
        }

        int finaiDamage = hitPart.isHead ? damage * 2 : damage;
        TakeDamage(finaiDamage);
    }

    private async UniTaskVoid HandleDeathAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        if (this != null) Destroy(gameObject);
    }
}
