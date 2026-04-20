using UnityEngine;
using UnityEngine.AI;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NpcView : MonoBehaviour, IInteractable
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private bool _hasInteracted = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
    }

    public void Interact(GameObject interactor)
    {
        if (_hasInteracted) return;
        _hasInteracted = true;

        bool isRescueSuccess = UnityEngine.Random.value > 0.5f;

        if (isRescueSuccess)
        {
            Debug.Log("生存者を救出しました");
            _animator.SetTrigger("Relieved");

            // スコア加算
            //SessionData.AddKill(5);

            DestroyAfterDelayAsync(3f).Forget();
        }
        else
        {
            Debug.Log("生存者がパニックになって逃げ出しました");
            _animator.SetTrigger("PanicRun");

            // 下の階層の方に逃げる
        }
    }

    private async UniTaskVoid DestroyAfterDelayAsync(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        if (this != null)
        {
            Destroy(gameObject);
        }
    }
}
