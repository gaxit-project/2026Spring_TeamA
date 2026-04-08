using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyView : MonoBehaviour
{

    private NavMeshAgent _agent;    // ナビゲーション制御
    private Animator _animator;

    public System.Action<Collider> OnContactStay;
    public System.Action<Collider> OffContactExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
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

    public void MoveTo(Vector3 direction)
    {
        _agent.destination = direction;    // ターゲットの現在地を目標値にセット
    }

    public void OnAttack()
    {
        _agent.isStopped = true;    // 追跡停止
        _animator.SetBool("Attack", true);
    }

    public void OffAttack()
    {
        _agent.isStopped = false;   // 追跡開始
        _animator.SetBool("Attack", false);
    }
}
