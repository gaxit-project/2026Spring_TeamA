using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCView : MonoBehaviour, IInteractable, IDamageable
{
    [SerializeField] private NPCData npcData;
    [SerializeField] private Transform escapePoint; // 逃げていく場所

    private Animator _animator;
    private NavMeshAgent _agent;
    private bool _hasInteracted = false;
    private bool _isDead = false;
    private bool _isPanicking = false; // パニック逃走中かどうか
    private CancellationTokenSource _destroyCts; // 消滅のタイマーをキャンセルできる

    public bool CanInteract => (!_hasInteracted || _isPanicking) && !_isDead;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _isDead = true;
        _isPanicking = false;

        CancelDestroyTimer();

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
        }

        _animator.SetTrigger("Die");

        UIManager.Instance.ShowNpcDeathMessage();
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract) return;

        if (npcData == null)
        {
            Debug.LogError("SurvivorNPC: NPCDataがセットされていません！");
            return;
        }

        if (_isPanicking)
        {
            _isPanicking = false;

            CancelDestroyTimer();

            _agent.isStopped = true;
            _agent.ResetPath();

            RescueSuccess(interactor);

            return;
        }

        _hasInteracted = true;

        // 助かるか逃げるか判定
        bool isRescueSuccess = UnityEngine.Random.value <= npcData.rescueSuccessProbability;

        if (isRescueSuccess)
        {
            RescueSuccess(interactor);
        }
        else
        {
            Debug.Log("生存者がパニックになって逃げ出しました");

            _isPanicking = true;

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

            _destroyCts = new CancellationTokenSource();

            DestroyAfterDelayAsync(npcData.destroyDelayAfterPanic, _destroyCts.Token).Forget();
        }
    }

    public string GetInteractPrompt()
    {
        // データが無かった場合
        if (UIManager.Instance == null || UIManager.Instance.textData == null) return "";


        if (_isPanicking)
        {
            // 逃走中
            return UIManager.Instance.textData.stopPanickingPrompt;
        }
        else
        {
            // 怯えている時
            return UIManager.Instance.textData.interactPrompt;
        }
    }

    private void RescueSuccess(GameObject interactor)
    {
        Debug.Log("生存者を救出しました");

        // プレイヤーの方を振り向く
        Vector3 lookPos = interactor.transform.position;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);

        _animator.CrossFade("Relieved", 0.1f);

        // スコア加算
        SessionData.AddRescue();

        UIManager.Instance.ShowRescueMessage();

        WalkAwayAsync(interactor).Forget();
    }

    private async UniTaskVoid WalkAwayAsync(GameObject interactor)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        if (this == null) return;

        _agent.isStopped = false;
        _agent.speed = 2f;

        _animator.CrossFade("Walk", 0.1f);

        Vector3 behindPlayer = interactor.transform.position - interactor.transform.forward * 5f;
        _agent.SetDestination(behindPlayer);

        DestroyAfterDelayAsync(npcData.destroyDelayAfterWalk).Forget();
    }

    private async UniTaskVoid DestroyAfterDelayAsync(float delay, CancellationToken token = default)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            if (this != null)
            {
                Destroy(gameObject);
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合は消滅しない
        }
    }

    private void CancelDestroyTimer()
    {
        if (_destroyCts != null)
        {
            _destroyCts.Cancel();
            _destroyCts.Dispose();
            _destroyCts = null;
        }
    }
}
