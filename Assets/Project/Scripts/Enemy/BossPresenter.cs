using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(BossView))]
public class BossPresenter : MonoBehaviour, IDamageable
{
    [SerializeField] private BossData data;
    
    private BossView view;

    private BossModel model;
    private CancellationTokenSource _stunCts;

    private void Awake()
    {
        view = GetComponent<BossView>();

        model = new BossModel(data);
        model.OnStateChanged += HandleStateChanged;
        view.SetMoveSpeed(data.moveSpeed);

        // NavMeshAgentの基本設定をデータから自動適用
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.stoppingDistance = data.stoppingDistance;
        }

        view.OnContactStay += HandlePlayerContact;
    }

    private void Start()
    {
        InitializeSequence().Forget();
    }

    /// <summary>
    /// 出現時のシーケンス（威嚇 -> AI開始）
    /// </summary>
    private async UniTaskVoid InitializeSequence()
    {
        view.StopMovement(true);
         view.PlayRoar();

        // 威嚇アニメーションの時間分だけ待機（例: 2.5秒）
        await UniTask.Delay(System.TimeSpan.FromSeconds(2.5f), cancellationToken: this.GetCancellationTokenOnDestroy());

        view.StopMovement(false);
        UpdateAILoop(this.GetCancellationTokenOnDestroy()).Forget();
    }

    /// <summary>
    /// AIのメインループ処理
    /// 状態に応じて挙動を決定する
    /// </summary>
    private async UniTaskVoid UpdateAILoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (model.CurrentState == BossModel.BossState.Chase)
            {
                if (PlayerPresenter.Instance != null)
                {
                    float dist = Vector3.Distance(transform.position, PlayerPresenter.Instance.transform.position);
                    
                    // 停止距離(stoppingDistance)より遠い場合のみ目的地を更新する
                    if (dist > data.stoppingDistance)
                    {
                        view.SetDestination(PlayerPresenter.Instance.transform.position);
                    }

                    // 攻撃距離判定
                    if (dist <= data.attackRange)
                    {
                        model.SetState(BossModel.BossState.Attack);
                    }
                }
            }
            await UniTask.Yield(token);
        }
    }


    /// <summary>
    /// 外部（プレイヤーなど）からのダメージ受信
    /// </summary>
    public void TakeDamage(int damage)
    {
        model.TakeDamage(damage);
    }

    private void HandleStateChanged(BossModel.BossState state)
    {
        switch (state)
        {
            case BossModel.BossState.Stunned:
                StartStunSequence().Forget();
                break;
            case BossModel.BossState.Attack:
                StartAttackSequence().Forget();
                break;
            case BossModel.BossState.Dead:
                view.StopMovement(true);
                view.PlayDie();
                break;
        }
    }

    /// <summary>
    /// ひざまずき状態のシーケンス管理
    /// </summary>
    private async UniTaskVoid StartStunSequence()
    {
        _stunCts?.Cancel();
        _stunCts = new CancellationTokenSource();

        view.StopMovement(true);
        view.SetKneeling(true);

        await UniTask.Delay(System.TimeSpan.FromSeconds(data.stunDuration), cancellationToken: _stunCts.Token);

        view.SetKneeling(false);
        await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f)); // 立ち上がりモーション待ち

        model.SetState(BossModel.BossState.Chase);
        view.StopMovement(false);
    }

    /// <summary>
    /// 攻撃アクションのシーケンス管理
    /// </summary>
    private async UniTaskVoid StartAttackSequence()
    {
        view.StopMovement(true);
        view.PlayAttack();

        // 攻撃の「振り」が終わるタイミングまで待機（例: 0.5秒後）
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        // プレイヤーがまだ射程内にいればダメージ実行
        if (PlayerPresenter.Instance != null)
        {
            float dist = Vector3.Distance(transform.position, PlayerPresenter.Instance.transform.position);
            if (dist <= data.attackRange)
            {
                PlayerPresenter.Instance.TakeDamage(data.attackDamage);
            }
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(data.attackInterval));

        if (model.CurrentState != BossModel.BossState.Stunned)
        {
            model.SetState(BossModel.BossState.Chase);
            view.StopMovement(false);
        }
    }


    private void HandlePlayerContact(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            // ゾンビと同じ方式でダメージを与える
            var playerView = col.GetComponent<PlayerView>();
            playerView?.OnHitByBoss?.Invoke(data);
        }
    }
}
