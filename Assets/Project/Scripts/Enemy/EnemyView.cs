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

    private float _idleEndTime = 0;
    float windowsTimer = 0f;

    private float sqrDistance = 0f;
    private bool canSee = false;
    private bool isPlayerWindow = false;
    private bool isHearing = false;

    private bool isWandering = false;
    [SerializeField] private float wanderRange = 10f;

    [Header("Idle Settings")]
    [SerializeField] private float minWaitTime = 1.0f; // 最小待ち時間
    [SerializeField] private float maxWaitTime = 3.0f; // 最大待ち時間

    [SerializeField] private float maxKnockTime = 1.0f;
    private float currentKnockTime = 0f;

    bool foundWindows = false;

    // 以下イベント定義
    public event System.Action<int, Collider> HitContact;

    // 以下行動アニメーション
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashKnock = Animator.StringToHash("Knock");
    private static readonly int HashTracking = Animator.StringToHash("Tracking");
    private static readonly int HashIsMoving = Animator.StringToHash("isMoving");

    private Vector3 windowHitPoint;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (enemyData != null && enemyData.controller != null)
        {
            _animator.runtimeAnimatorController = enemyData.controller; // アニメーション差し替え適用
        }

        if(enemyData != null &&　_agent != null)
        {
            _agent.speed = enemyData.moveSpeed;
            _agent.acceleration = enemyData.moveSpeed * 2f;
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
        if (enemyData != null && _agent != null)
        {
            _agent.speed = enemyData.moveSpeed;
        }

        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool(HashIsMoving, isMoving);
    }

    public void Moving()
    {
        if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh || isHit)
        {
            return;
        }

        ScanEnvironment();  // 状況データ収集
        EnemyState nextState = DetermineNextState();    // 状態判断
        ChangeState(nextState);  // 状態変化
        CurrentAction();    // 実行
    }

    private void ScanEnvironment()
    {
        foundWindows = false;
        isPlayerWindow = false;
        canSee = false;

        Vector3 startPos = transform.position + Vector3.up * 4.5f;
        Vector3 offset = player.position + Vector3.up * 4.5f - startPos;
        Vector3 dir = offset.normalized;

        RaycastHit[] hits = Physics.SphereCastAll(startPos, 0.4f, dir, enemyData.detectionRange);  // レイにあたったコライダーを全て取得
        Debug.DrawRay(startPos, dir * enemyData.detectionRange, Color.red);

        float playerDistance = float.MaxValue;
        float windowDistance = float.MaxValue;
        float othersDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            float dist = hits[i].distance;
            Collider col = hits[i].collider;

            if (hits[i].collider.CompareTag("Player"))
            {
                playerDistance = dist;  // プレイヤーの距離取得
            }
            else if (hits[i].collider.CompareTag("WindowZone"))
            {
                if (dist < windowDistance)
                {
                    windowDistance = dist;  // 窓の距離
                    windowHitPoint = (dist > 0) ? hits[i].point : col.ClosestPoint(transform.position + Vector3.up * 1.5f);
                    foundWindows = true;
                }
            }
            else if (!hits[i].collider.isTrigger)
            {
                if (othersDistance > dist)
                {
                    othersDistance = dist;
                }
            }
        }

        if(foundWindows && windowDistance < playerDistance)
        {
            isPlayerWindow = true;
            return;
        }
        if(playerDistance < othersDistance)
        {
            canSee = true;
        }
    }

    private EnemyState DetermineNextState()
    {
        Vector3 offset = player.position - transform.position;
        sqrDistance = offset.sqrMagnitude;

        if (canSee && sqrDistance <= enemyData.attackDistance * enemyData.attackDistance)  // 攻撃
        {
            return EnemyState.Attacking;
        }
        if (canSee || isHearing)    // 追跡
        {
            return EnemyState.Tracking; ;
        }
        if (isPlayerWindow)  // 窓叩き
        {
            if(currentState == EnemyState.Knock)
            {
                currentKnockTime += Time.deltaTime;
            }

            if(currentKnockTime >= maxKnockTime)
            {
                isPlayerWindow = false;
                currentKnockTime = 0;
                return EnemyState.Tracking;
            }

            float distToWindow = Vector3.Distance(transform.position, windowHitPoint);
            if (distToWindow <= enemyData.windowKnockDistance)
            {
                return EnemyState.Knock;
            }
            return EnemyState.Tracking;
        }

        currentKnockTime = 0f;

        return EnemyState.Idle;
    }

    private void ChangeState(EnemyState nextState)
    {
        if (currentState == nextState || isHit)
        {
            return;
        }

        // 状態が変わった瞬間だけログを出す
        Debug.Log($"<color=yellow>[StateChange]</color> {currentState} -> {nextState}");

        if (nextState == EnemyState.Attacking)
        {
            SoundManager.Instance.PlaySound(0);
        }

        if (nextState == EnemyState.Tracking)
        {
            SoundManager.Instance.PlaySound(1);
        }

        currentState = nextState;

        _animator.SetBool(HashAttack, currentState == EnemyState.Attacking);
        _animator.SetBool(HashKnock, currentState == EnemyState.Knock);
        _animator.SetBool(HashTracking, currentState == EnemyState.Tracking);

        if(isHit)
        {
            _agent.isStopped = (currentState == EnemyState.Attacking || currentState == EnemyState.Knock);
        }
    }

    private void CurrentAction()
    {
        switch (currentState)
        {
            case EnemyState.Tracking:
                _agent.isStopped = false;
                _agent.destination = isPlayerWindow ? windowHitPoint : player.position;
                break;
            case EnemyState.Attacking:
                break;
            case EnemyState.Knock:
                _agent.ResetPath();
                SoundManager.Instance.PlaySound(2);
                break;
            case EnemyState.Idle:
                Idle();
                break;
        }
    }

    public void SetHearing(bool value) => isHearing = value;
    public void MoveTo(Vector3 position) => _agent.SetDestination(position);

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
            isWandering = true;
        }
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
        if(isHit)
        {
            return;
        }
        isHit = true;

        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.updateRotation = false;

        _animator.SetTrigger("GetHit");

        Vector3 knockback = (transform.position - player.position).normalized;  // ノックバック
        float knockbackDistance = 1.0f;
        float knockBackTime = 0;

        while(knockBackTime<0.15f)
        {
            _agent.Move(knockback * (knockbackDistance / 0.15f) * Time.deltaTime);
            knockBackTime += Time.deltaTime;
            await UniTask.Yield();
        }

        await UniTask.Delay(300);
        _agent.updateRotation = true;
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
        if (skinrenderer == null)
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