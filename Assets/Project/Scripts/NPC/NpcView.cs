using UnityEngine;
using UnityEngine.AI;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NpcView : MonoBehaviour, IInteractable
{
    [SerializeField] private NpcData npcData;
    [SerializeField] private Transform escapePoint; // 逃げていく場所

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

        if (npcData == null)
        {
            Debug.LogError("SurvivorNPC: NPCDataがセットされていません！");
            return;
        }

        _hasInteracted = true;

        // 助かるか逃げるか判定
        bool isRescueSuccess = UnityEngine.Random.value <= npcData.rescueSuccessProbability;

        if (isRescueSuccess)
        {
            Debug.Log("生存者を救出しました");

            // プレイヤーの方を振り向く
            Vector3 lookPos = interactor.transform.position;
            lookPos.y = transform.position.y;

            transform.LookAt(lookPos);

            _animator.SetTrigger("Relieved");

            // スコア加算
            //SessionData.AddKill(5);

            WalkAwayAsync(interactor).Forget();
        }
        else
        {
            Debug.Log("生存者がパニックになって逃げ出しました");
            _animator.SetTrigger("PanicRun");
            _agent.isStopped = false;
            _agent.speed = npcData.panicRunSpeed;

            // 下の階層の方に逃げる
            if (escapePoint != null)
            {
                _agent.SetDestination(escapePoint.position);
            }
            else
            {
                Vector3 runDirection = (transform.position - interactor.transform.position).normalized;
                Vector3 targetPos = transform.position + runDirection * npcData.panicRunDistance;
                _agent.SetDestination(targetPos);
            }

            DestroyAfterDelayAsync(npcData.destroyDelayAfterPanic).Forget();
        }
    }

    private async UniTaskVoid WalkAwayAsync(GameObject interactor)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        if (this == null) return;

        _agent.isStopped = false;
        _agent.speed = 2f;

        Vector3 behindPlayer = interactor.transform.position - interactor.transform.forward * 5f;
        _agent.SetDestination(behindPlayer);

        DestroyAfterDelayAsync(npcData.destroyDelayAfterWalk).Forget();
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
