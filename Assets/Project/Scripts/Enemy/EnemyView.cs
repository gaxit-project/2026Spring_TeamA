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

    [SerializeField] private AudioClip[] _voices;
    private float _volume = 0.5f;

    // 以下イベント定義
    public event System.Action OnAttackHitEvent;
    public event System.Action<int, Collider, EnemyBodyPart.HitPartType> HitContact;

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
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        _agent.avoidancePriority = UnityEngine.Random.Range(0, 99);
    }

    private void Update()
    {
        if (Time.frameCount % 4 != 0) return;
        if (isHit) return;
        if(_agent != null && _agent.isOnNavMesh)
        {
            _animator.SetBool(HashIsMoving, _agent.velocity.sqrMagnitude > 0.1f);
        }
    }

    public void Moving()
    {
        if (isHit) return;
        if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh) return;

        ScanEnvironment();  // 状況データ収集
        EnemyState nextState = DetermineNextState();    // 状態判断
        ChangeState(nextState);  // 状態変化
        CurrentAction();    // 実行
    }

    private void ScanEnvironment()
    {
        isPlayerWindow = false;
        canSee = false;

        Vector3 startPos = transform.position + Vector3.up * 4.5f;
        Vector3 offset = player.position + Vector3.up * 4.5f - startPos;
        Vector3 dir = offset.normalized;

        Debug.DrawRay(startPos, dir * enemyData.detectionRange, Color.red);

        if (Physics.Raycast(startPos, dir, out RaycastHit hit, enemyData.detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                canSee = true;
            }
            else if (hit.collider.CompareTag("WindowZone"))
            {
                isPlayerWindow = true;
                windowHitPoint = hit.point;
            }
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
        // 要修正
        Debug.Log($"<color=yellow>[StateChange]</color> {currentState} -> {nextState}");

        if (this == null) return;
        if (nextState == EnemyState.Attacking)
        {
            _volume = SoundManager.Instance.GetSEVolume();
            //AudioSource.PlayClipAtPoint(_voices[0], transform.position, _volume);
            SoundManager.Instance.PlaySound(0);
        }

        if (nextState == EnemyState.Tracking)
        {
            _volume = SoundManager.Instance.GetSEVolume();
            //AudioSource.PlayClipAtPoint(_voices[1], transform.position, _volume);
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

        if (nextState == EnemyState.Attacking || nextState == EnemyState.Knock || nextState == EnemyState.Idle)
        {
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
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
    /// アニメーションイベントから呼び出される攻撃ヒットタイミング
    /// </summary>
    public void OnAttackHit()
    {
        OnAttackHitEvent?.Invoke();
    }

    /// <summary>
    /// ダメージ判定
    /// </summary>
    public void ReceiveDamage(int damage, Collider collider, EnemyBodyPart.HitPartType partType)
    {
        HitContact?.Invoke(damage, collider, partType);
    }

    public async UniTask Hit(EnemyBodyPart.HitPartType partType)
    {
        if (isHit) return;
        isHit = true;

        _agent.isStopped = true;
        _agent.ResetPath();

        _animator.SetInteger("HitPart", (int)partType);
        _animator.SetTrigger("GetHit");

        await UniTask.Yield();
        
        _agent.updateRotation = false;
       
        Vector3 knockback = (transform.position - player.position).normalized;  // ノックバック
        float knockbackDistance = 2.0f;
        float knockBackTime = 0;

        while(knockBackTime<0.15f)
        {
            _agent.Move(knockback * (knockbackDistance / 0.15f) * Time.deltaTime);
            knockBackTime += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 被弾によるひるみ状態が終了したときに呼び出され、移動・回転を再開する（アニメーションイベント用）。
    /// </summary>
    public void EndHit()
    {
        if (this == null) return;

        // 被弾フラグを解除し、追跡・回転・移動を再開
        isHit = false;
        _agent.updateRotation = true;
        _agent.isStopped = false;
    }

    /// <summary>
    /// animatorの再生速度と移動速度の変更
    /// </summary>
    /// <param name="speed">変更後のanimator再生速度</param>
    public void ChangeSpeed(float speed)
    {
        Debug.LogWarning("Change speed : " + speed + "f");
        _animator.speed = speed;
        _agent.speed = _agent.speed * speed;
        _agent.acceleration = _agent.acceleration * speed;
    }

    public void Die()
    {
        if(_animator.speed != 1) _animator.speed = 1;

        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetBool("Die", true);
    }

    public async UniTask Extinction()
    {
        if (renderers == null) return;

        float duration = 2.0f;
        float time = 0f;

        while (time < duration)
        {
            if (this == null) return;
            time += Time.deltaTime;
            float alpha = 1.0f - time / duration;
            foreach (var r in renderers)
            {
                if(r == null || r.material == null) continue;
                Color color = r.material.color;
                color.a = alpha;
                r.material.color = color;
            }
            await UniTask.Yield();  // 1フレーム待機
        }
    }

    public void SeyActiveLogic(bool active)
    {
        if (_agent.isOnNavMesh) _agent.isStopped = !active;
        _agent.enabled = active;
        _animator.enabled = active;
    }
}