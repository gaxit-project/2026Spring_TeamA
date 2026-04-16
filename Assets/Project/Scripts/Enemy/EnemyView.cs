using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyView : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;   // ScriptableObject

    private NavMeshAgent _agent;    // 経路探索・移動制御
    private Animator _animator; // アニメーション制御

    public Transform player;    // プレイヤーの位置情報
    public System.Action<Collider> OnContactStay;
    public System.Action<Collider> OffContactExit;
    public System.Action<Collider, Collider> HitContact;
    public System.Action<Vector3> OnFoundPlayer;

    public float fieldOfView = 60f; // 視野角度
    public float detectionRange = 10f;  // 検出範囲
    public bool isTracking = false;    // 追跡

    private bool isHearing = false;
    private bool isHit = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        // プレイヤーのTransformを自動取得
        if (player == null && PlayerPresenter.Instance != null)
        {
            player = PlayerPresenter.Instance.PlayerView.transform;
        }
    }

    private void Update()
    {
        if (player == null || enemyData == null)
        {
            return;
        }

        Vector3 startPos = transform.position + Vector3.up;
        Vector3 offset = player.position - transform.position;
        Vector3 dir = offset.normalized;
        float sqrDistance = offset.sqrMagnitude;    // 2点間の距離の2乗

        // 視界にプレイヤーが入っているか
        bool isInView = offset.sqrMagnitude <= detectionRange * detectionRange && Vector3.Dot(transform.forward, dir) >= Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);

        // プレイヤーを視認したかどうか(遮蔽物判定)
        bool canSee = isInView && Physics.Raycast(startPos, dir, out var hit, detectionRange) && hit.collider.CompareTag("Player");

        Debug.DrawRay(startPos, dir * detectionRange, canSee ? Color.red : Color.gray);

        // 攻撃範囲内にいるとき
        if (sqrDistance <= enemyData.attackDistance * enemyData.attackDistance)   
        {
            _agent.isStopped = true;    // 追跡停止
            _animator.SetBool("Attack", true);
            isTracking = false;
        }
        else if (canSee || isHearing || isHit) // 追跡開始
        {
            _animator.SetBool("Attack", false);
            OnFoundPlayer?.Invoke(player.position);
            isHearing = false;
        }
        else if (isTracking) // 追跡中
        {
            _animator.SetBool("Attack", false);
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                isTracking = false;
                _agent.isStopped = true;
            }
        }
        else
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _animator.SetBool("Attack", false);
        }
    }

    /// <summary>
    /// アタック開始処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        OnContactStay?.Invoke(other);   // 通知
    }

    /// <summary>
    /// アタック終了処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        OffContactExit?.Invoke(other);   // 通知
    }

    public void SetHearing(bool value)
    {
        isHearing = value;
    }

    public void MoveTo(Vector3 direction)
    {
        if (isTracking)
        {
            _agent.isStopped = false;
            _agent.destination = direction;    // ターゲットの現在地を目標値にセット
        }
    }

    public async UniTask Hit()
    {
        isHit = true;
        _agent.isStopped = true;
        await UniTask.Delay(150);
        _agent.isStopped = false;
        isHit = false;
    }

    public void Die()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetBool("Die", true);
    }
}
