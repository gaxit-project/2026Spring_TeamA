using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BossView : MonoBehaviour
{
    public event System.Action<Collider> OnContactStay;

    private NavMeshAgent agent;
    private Animator animator;

    // アニメーションパラメータのハッシュ化
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int IsKneeling = Animator.StringToHash("IsKneeling");
    private static readonly int RoarTrigger = Animator.StringToHash("Roar");
    private static readonly int DieTrigger = Animator.StringToHash("Die");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// ナビメッシュエージェントの速度を設定
    /// </summary>
    public void SetMoveSpeed(float speed) => agent.speed = speed;

    /// <summary>
    /// 目的地を設定
    /// </summary>
    public void SetDestination(Vector3 target) => agent.SetDestination(target);
    
    /// <summary>
    /// 移動の停止/再開を制御
    /// </summary>
    public void StopMovement(bool stop) => agent.isStopped = stop;

    /// <summary>
    /// 威嚇（咆哮）アニメーションを再生
    /// </summary>
    public void PlayRoar() => animator.SetTrigger(RoarTrigger);

    /// <summary>
    /// ひざまずき（ダウン）アニメーションの制御
    /// </summary>
    public void SetKneeling(bool kneeling) => animator.SetBool(IsKneeling, kneeling);
    
    /// <summary>
    /// 振りかぶり攻撃のアニメーション再生
    /// </summary>
    public void PlayAttack() => animator.SetTrigger(AttackTrigger);
    
    /// <summary>
    /// 死亡アニメーションの再生
    /// </summary>
    public void PlayDie() => animator.SetTrigger(DieTrigger);
    
    /// <summary>
    /// アニメーションの更新（移動速度同期）
    /// </summary>
    private void Update()
    {
        animator.SetFloat(Speed, agent.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnContactStay?.Invoke(other);
    }
}
