using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using DG.Tweening;
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

        DisableColliderDelayedAsync().Forget();

        _animator.SetTrigger("Die");

        UIManager.Instance.ShowNpcDeathMessage();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractPrompt();
        }
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
            _agent.velocity = Vector3.zero;

            // 逃走後に助けた時のセリフ
            UIEvents.OnShowSystemMessage?.Invoke(UIManager.Instance.textData.npcRescueAfterEscapeMessage, 3.0f);

            PlayerPresenter.Instance.SetInputBlocked(true);
            RescueSuccess(interactor);

            return;
        }

        _hasInteracted = true;

        InteractSequenceAsync(interactor).Forget();
    }

    private async UniTaskVoid InteractSequenceAsync(GameObject interactor)
    {
        PlayerPresenter.Instance.SetInputBlocked(true);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        if (this == null || _isDead)
        {
            if (PlayerPresenter.Instance != null) PlayerPresenter.Instance.SetInputBlocked(false);
            return;
        }

        bool isRescueSuccess = UnityEngine.Random.value <= npcData.rescueSuccessProbability;

        if (isRescueSuccess)
        {
            // 最初から助かった時のセリフ
            UIEvents.OnShowSystemMessage?.Invoke(UIManager.Instance.textData.npcInitialRescueMessage, 3.0f);
            
            RescueSuccess(interactor);
        }
        else
        {
            _isPanicking = true;

            // 逃走する時のセリフ
            UIEvents.OnShowSystemMessage?.Invoke(UIManager.Instance.textData.npcPanicMessage, 3.0f);

            Debug.Log("生存者がパニックになって逃げ出しました");
            _animator.SetTrigger("PanicRun");
            _agent.isStopped = false;
            _agent.speed = npcData.panicRunSpeed;

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
            
            // パニック時はディレイ後に1.5秒かけてフェードアウト
            float delay = Mathf.Max(0, npcData.destroyDelayAfterPanic - 1.5f);
            FadeOutAndDestroyAsync(delay, 1.5f, _destroyCts.Token).Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            if (PlayerPresenter.Instance != null)
            {
                PlayerPresenter.Instance.SetInputBlocked(false);
            }
        }
    }

    public string GetInteractPrompt()
    {
        if (_isDead) return "";
        if (UIManager.Instance == null || UIManager.Instance.textData == null) return "";

        if (_isPanicking)
        {
            return UIManager.Instance.textData.stopPanickingPrompt;
        }
        else
        {
            return UIManager.Instance.textData.interactPrompt;
        }
    }

    private void RescueSuccess(GameObject interactor)
    {
        Debug.Log("生存者を救出しました");

        // コライダーを無効化して貫通させる
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        
        // Rigidbodyがついていたら、すり抜けて床下に落ちないようにKinematicにする
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Vector3 lookPos = interactor.transform.position;
        lookPos.y = transform.position.y;
        
        // 滑らかに振り向く
        if (!_isPanicking){
            transform.DOLookAt(lookPos, 0.5f);
        }
        else{
            transform.DOLookAt(lookPos, 0.1f);
        }

        _animator.CrossFade("Relieved Sigh", 0.1f);

        SessionData.AddRescue();

        WalkAwayAsync(interactor).Forget();
    }

    private async UniTaskVoid WalkAwayAsync(GameObject interactor)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            if (PlayerPresenter.Instance != null) PlayerPresenter.Instance.SetInputBlocked(false);
            return;
        }

        if (PlayerPresenter.Instance != null)
        {
            PlayerPresenter.Instance.SetInputBlocked(false);
        }

        if (this == null || _isDead) return;

        _agent.isStopped = false;
        _agent.speed = 2f;

        _animator.CrossFade("Walking", 0.1f);

        Vector3 behindPlayer = interactor.transform.position - interactor.transform.forward * 5f;
        _agent.SetDestination(behindPlayer);

        // 歩き去る時はディレイ後に1.5秒かけてフェードアウト
        float delay = Mathf.Max(0, npcData.destroyDelayAfterWalk - 1.5f);
        FadeOutAndDestroyAsync(delay, 1.5f).Forget();
    }

    private async UniTaskVoid DisableColliderDelayedAsync()
    {
        // 弾がNPCに飛んでくるまでの時間稼ぎ
        await UniTask.Delay(System.TimeSpan.FromSeconds(2.0f));

        // 死亡後はインタラクトできないようコライダーを無効化
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Rigidbodyがついていたら、すり抜けて床下に落ちないようにKinematicにする
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void SetupMaterialForFade(Material mat)
    {
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 2); // Fade
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.SetShaderPassEnabled("ShadowCaster", false);
        }
    }

    private async UniTaskVoid FadeOutAndDestroyAsync(float delay, float fadeDuration, CancellationToken token = default)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            if (this == null) return;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    SetupMaterialForFade(mat);

                    if (mat.HasProperty("_Color"))
                    {
                        mat.DOFade(0f, fadeDuration);
                    }
                    else if (mat.HasProperty("_BaseColor"))
                    {
                        mat.DOFade(0f, "_BaseColor", fadeDuration);
                    }
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: token);

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
