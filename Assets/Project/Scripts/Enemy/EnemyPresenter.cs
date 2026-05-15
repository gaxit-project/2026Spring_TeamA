using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyPresenter : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyView view;    // 操作対象のview
    [SerializeField] private EnemyData enemyData;   // ScriptableObject
    [SerializeField] private GunData gunData;   // ScriptableObject
    [SerializeField] private List<EnemyBodyPart> bodyParts;  // 当たり判定リスト
    [SerializeField] private SoundDetectionView soundView;
    [SerializeField] private float sleepDistance = 50f; // アクティブ距離

    private EnemyModel model;

    private Transform _target;     // 追跡対象
    private bool _isDead = false;   // 死亡判定
    private CancellationTokenSource _cts = new CancellationTokenSource();

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

        view.HitContact += OnHit; 
        view.OnAttackHitEvent += HandleAttackHit; 

        if (soundView == null)
        {
            soundView = SoundDetectionView.Instance;
        }

        if (soundView != null)
        {
            soundView.HitEnemy += HandleSoundDetected;
        }
    }

    private void Start()
    {
        if (PlayerPresenter.Instance != null)
        {
            _target = PlayerPresenter.Instance.PlayerView.transform;
        }
        MoveInterval(_cts.Token).Forget();
    }

    private void OnDestroy()
    {
        if(view != null)
        {
            view.HitContact -= OnHit;
            view.OnAttackHitEvent -= HandleAttackHit;
        }

        if(soundView != null)
        {
            soundView.HitEnemy -= HandleSoundDetected;
        }

        if (_cts != null)
        {
            _cts.Cancel(); // 万が一キャンセルされていない時の保険
            _cts.Dispose();
            _cts = null;
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

            _cts.Cancel();

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

    /// <summary>
    /// 攻撃アニメーションのヒットタイミングに合わせてプレイヤーにダメージを与える
    /// </summary>
    private void HandleAttackHit()
    {
        if (_isDead) return;
        if (PlayerPresenter.Instance == null) return;

        // プレイヤーとの距離を確認
        float dist = Vector3.Distance(transform.position, PlayerPresenter.Instance.transform.position);

        if (dist <= enemyData.attackDistance)
        {
            PlayerPresenter.Instance.TakeDamage(enemyData.enemyAttackPower);

            // プレイヤーにノックバック効果を適用（後ろに弾く）
            Vector3 pushDirection = (PlayerPresenter.Instance.transform.position - transform.position).normalized;
            pushDirection.y = 0f; // Y軸は固定
            PlayerPresenter.Instance.PlayerView.ApplyKnockback(pushDirection * 2f, 0.15f);
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <returns></returns>
    private async UniTask MoveInterval(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.value), cancellationToken: token);

        while(!token.IsCancellationRequested)
        {
            float distSqr = (transform.position - _target.position).sqrMagnitude;
            float sleepThresholdSqr = sleepDistance * sleepDistance;
            
            if(distSqr > sleepThresholdSqr)
            {
                view.SeyActiveLogic(false);

                await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: token);
                continue;
            }

            view.SeyActiveLogic(true);
            view.Moving();

            float nextInterval = UnityEngine.Random.Range(0.8f, 1.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(nextInterval), cancellationToken: token);
        }
    }

    /// <summary>
    /// ゾンビが消える処理
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid HandleDeathAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        await view.Extinction();
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        if (this != null) Destroy(gameObject);
    }
}
