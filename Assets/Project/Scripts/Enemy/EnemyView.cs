using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyView : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;   // ScriptableObject
    private NavMeshAgent _agent;    // 経路探索・移動制御
    private Animator _animator; // アニメーション制御
    private Renderer[] renderers; 
    public Transform player;    // プレイヤーの位置情報
    public bool isTracking { get; set; } = false;
    private bool isHit = false;

    public enum EnemyState { Idle, Tracking, Attacking, Knock }

    private EnemyState currentState = EnemyState.Idle;

    private float sqrDistance = 0f;
    private bool canSee = false;
    private bool isPlayerWindow = false;
    private bool isHearing = false;

    private bool isWandering = false;
    [SerializeField] private float wanderRange = 10f;

    [Header("Idle Settings")]
    [SerializeField] private float minWaitTime = 1.0f; // 最小待ち時間
    [SerializeField] private float maxWaitTime = 3.0f; // 最大待ち時間

    // 以下イベント定義
    public event System.Action<Collider> OnContactStay;
    public event System.Action<Collider> OffContactExit;
    public event System.Action<Vector3> OnFoundPlayer;
    public event System.Action<int, Collider> HitContact;

    // 以下行動アニメーション
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashKnock = Animator.StringToHash("Knock");
    private static readonly int HashTracking = Animator.StringToHash("Tracking");
    private static readonly int HashIsMoving = Animator.StringToHash("isMoving");

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (enemyData != null && _agent != null)
        {
            _agent.speed = enemyData.moveSpeed;
        }

        // プレイヤーのTransformを自動取得
        if (player == null && PlayerPresenter.Instance != null)
        {
            player = PlayerPresenter.Instance.PlayerView.transform;
        }

        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool(HashIsMoving, isMoving);
    }

    public void Moving()
    {
        ScanEnvironment();  // 状況データ収集
        EnemyState nextState = DetermineNextState();    // 状態判断
        ChangeState(nextState);  // 状態変化
        CurrentAction();    // 実行
    }

    private void ScanEnvironment()
    {
        bool isInView = false;  // プレイヤーが見えているかどうか
        Vector3 startPos = transform.position + Vector3.up;
        Vector3 offset = player.position - transform.position;
        Vector3 dir = offset.normalized;
        sqrDistance = offset.sqrMagnitude;    // 2点間の距離の2乗

        if (Physics.Raycast(startPos, dir, out var hit, enemyData.detectionRange))
        {
            // 視界にプレイヤーが入っているか
            isInView = offset.sqrMagnitude <= enemyData.detectionRange * enemyData.detectionRange && Vector3.Dot(transform.forward, dir) >= Mathf.Cos(enemyData.fieldOfView * 0.5f * Mathf.Deg2Rad);
            canSee = isInView && hit.collider.CompareTag("Player"); // 遮蔽物判定
        }
        else
        { 
            canSee = false; 
        }
    }

    private EnemyState DetermineNextState()
    {
        if(sqrDistance <= enemyData.attackDistance * enemyData.attackDistance)  // 攻撃
        {
            return EnemyState.Attacking;
        }
        if(isPlayerWindow)  // 窓叩き
        {
            return EnemyState.Knock;
        }
        if(canSee || isHearing || isTracking) // 追跡
        {
            return EnemyState.Tracking;
        }
        return EnemyState.Idle;
    }

    private void ChangeState(EnemyState nextState)
    {
        if(currentState == nextState)
        {
            return;
        }

        currentState = nextState;

        _animator.SetBool(HashAttack, currentState == EnemyState.Attacking);
        _animator.SetBool(HashKnock, currentState == EnemyState.Knock);
        _animator.SetBool(HashTracking, currentState == EnemyState.Tracking);

        _agent.isStopped = (currentState == EnemyState.Attacking || currentState == EnemyState.Knock);
    }

    private void CurrentAction()
    {
        switch (currentState)
        {
            case EnemyState.Attacking:
                break;
            case EnemyState.Knock:
                break;
            case EnemyState.Tracking:
                _agent.destination = player.position;
                break;
            case EnemyState.Idle:
                Idle();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnContactStay?.Invoke(other);

        if (other.CompareTag("WindowZone"))
        {
            isPlayerWindow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OffContactExit?.Invoke(other);   // 通知

        if (other.CompareTag("WindowZone"))
        {
            isPlayerWindow = false;
        }
    }

    public void SetHearing(bool value) => isHearing = value;
    public void MoveTo(Vector3 position) => _agent.SetDestination(position);

    // クラスのメンバ変数として追加（以前の currentTimer は削除または不要になります）
    private float _idleEndTime = 0f;

    private void Idle()
    {
        // 徘徊中かチェック
        if (isWandering)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
            {
                isWandering = false;
                _idleEndTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
            }
            return;
        }

        if (Time.time < _idleEndTime)   // 待機
        {
            return;
        }

        // 徘徊処理
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRange;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRange, NavMesh.AllAreas))
        {
            _agent.isStopped = false;
            _agent.SetDestination(hit.position);
            isWandering = true;        }
    }

    /// <summary>
    /// ダメージ判定
    /// </summary>
    public void ReceiveDamage(int damage, Collider collider)
    {
        HitContact?.Invoke(damage, collider);
    }

    public async UniTask Hit()
    {
        isHit = true;
        _agent.isStopped = true;
        _animator.SetTrigger("GetHit");
        await UniTask.Delay(50);
        _agent.isStopped = false;
        isHit = false;
    }

    public void Die()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetBool("Die", true);
    }

    public async UniTask Extinction()
    {
        var skinrenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if(skinrenderer == null)
        {
            return;
        }

        var material = skinrenderer.material;   

        float duration = 2.0f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = 1.0f - time / duration;

            foreach (var r in renderers)
            {
                Color color = r.material.color;
                color.a = alpha;
                r.material.color = color;
            }
            await UniTask.Yield();  // 1フレーム待機
        }
    }
}