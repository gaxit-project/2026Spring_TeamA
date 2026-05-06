using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BossView : MonoBehaviour
{
    public event System.Action<Collider> OnContactStay;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip roarClip;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip attackClip;

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
    /// 足音を再生（アニメーションイベント用）
    /// </summary>
    public void PlayFootstep()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }

    /// <summary>
    /// 威嚇（咆哮）アニメーションとSEを再生
    /// </summary>
    public void PlayRoar()
    {
        animator.SetTrigger(RoarTrigger);
        if (audioSource != null && roarClip != null)
        {
            audioSource.PlayOneShot(roarClip);
        }
    }

    /// <summary>
    /// ひざまずき（ダウン）アニメーションの制御
    /// </summary>
    public void SetKneeling(bool kneeling) => animator.SetBool(IsKneeling, kneeling);
    
    /// <summary>
    /// 振りかぶり攻撃のアニメーション再生
    /// </summary>
    public void PlayAttack() => animator.SetTrigger(AttackTrigger);

    /// <summary>
    /// 攻撃音を再生（アニメーションイベント等用）
    /// </summary>
    public void PlayAttackSound()
    {
        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(attackClip);
        }
    }

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