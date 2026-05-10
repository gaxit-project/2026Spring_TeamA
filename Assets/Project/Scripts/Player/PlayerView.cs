using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerView : MonoBehaviour
{
    // EnemyDataを受け取るためのアクション
    public System.Action<EnemyData> OnHitByEnemy;
    public System.Action<BossData> OnHitByBoss;

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int AimPitchHash = Animator.StringToHash("AimPitch");
    private static readonly int FireTrigger = Animator.StringToHash("OnFire");
    private static readonly int ReloadTrigger = Animator.StringToHash("OnReload");

    private Rigidbody rb;
    private bool isKnockedBack;
    private float knockbackTimer;

    /// <summary>
    /// 物理演算（velocity）を使用して移動を行う
    /// </summary>
    public void Move(Vector3 move)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        // ノックバック中の処理
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return; // 吹き飛ばされている間は入力を受け付けない
        }

        if (Time.deltaTime > 0f)
        {
            // Presenter側でdeltaTimeが掛けられているため、速度(m/s)に逆算して代入
            Vector3 targetVelocity = transform.TransformDirection(move) / Time.deltaTime;
            
            // 重力による落下(Y軸の速度)は維持する
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }

        if (animator != null)
        {
            bool moving = move.magnitude > 0.001f;
            animator.SetBool(IsMoving, moving);
        }
    }

    /// <summary>
    /// 外部からプレイヤーにノックバックを適用する
    /// </summary>
    public void ApplyKnockback(Vector3 force, float duration)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        isKnockedBack = true;
        knockbackTimer = duration;
        rb.linearVelocity = force;
    }

    public void SetDashAnimation(bool isDashing)
    {
        animator.SetBool("IsDashing", isDashing);
    }

    public void UpdateBodyRotation(float panAngle)
    {
        transform.rotation = Quaternion.Euler(0, panAngle, 0);
    }

    public void SetUpperBodyPitch(float pitch)
    {

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        if (animator != null)
        {
            animator.SetFloat(AimPitchHash, pitch);
        }
    }

    public void SetAnimatorController(RuntimeAnimatorController newController)
    {
        if (animator != null && newController != null)
        {
            animator.runtimeAnimatorController = newController;
        }
    }

    public void SetAiming(bool aiming)
    {
        if (animator != null) animator.SetBool(IsAimingHash, aiming);
    }

    public void PlayFireAnim()
    {
        if (animator != null) animator.SetTrigger(FireTrigger);
    }

    public void PlayReloadAnim()
    {
        if (animator != null)
        {
            //animator.SetTrigger(ReloadTrigger);
        }
    }

    public void OnShoot()
    {
        OnFireEffectTiming?.Invoke();
    }

    public void PlayDieAnim()
    {
        if (animator != null) animator.SetTrigger("Die");
    }

    public void PlayTimeOutAnim()
    {
        if (animator != null) animator.SetTrigger("TimeLimit");
    }

    public void ForceIdle()
    {
        if (animator != null)
        {
            // 移動フラグをオフにする
            animator.SetBool("IsDashing", false);
            animator.SetBool(IsMoving, false);
            animator.Play("Idle", 0, 0f);
        }
    }

    // 入力があったことをPresenterに知らせるためのイベント
    public event System.Action<Vector2> OnMoveInputReceived;
    public event System.Action<Vector2> OnLookInputReceived;

    public event System.Action<bool> OnDashInputReceived;
    public event System.Action<bool> OnAimInputReceived;
    public event System.Action<bool> OnFireInputReceived;
    
    public event System.Action OnFireEffectTiming;
    public event System.Action OnReloadInputReceived;
    public event System.Action OnInteractInputReceived;
    
    public event System.Action<int> OnWeaponSwitchInputRecieved;
    public event System.Action<int> OnWeaponDirectSelect;
    
    public event System.Action<Collider> OnTriggerEnterEvent;
    public event System.Action<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    private void OnMove(InputValue value)
    {
        // 入力値を読み取って、イベントを購読している先に通知する
        OnMoveInputReceived?.Invoke(value.Get<Vector2>());
    }

    private void OnDash(InputValue value)
    {
        OnDashInputReceived?.Invoke(value.isPressed);
    }

    private void OnLook(InputValue value)
    {
        OnLookInputReceived?.Invoke(value.Get<Vector2>());
    }

    private void OnAim(InputValue value)
    {
        OnAimInputReceived?.Invoke(value.isPressed);
    }

    private void OnFire(InputValue value)
    {
        OnFireInputReceived?.Invoke(value.isPressed);
    }

    private void OnReload(InputValue value)
    {
        OnReloadInputReceived?.Invoke();
    }

    private void OnNextWeapon(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitchInputRecieved?.Invoke(1);
    }

    private void OnPrevWeapon(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitchInputRecieved?.Invoke(-1);
    }

    private void OnWeapon1(InputValue value)
    {
        if (value.isPressed) OnWeaponDirectSelect?.Invoke(0);
    }

    private void OnWeapon2(InputValue value)
    {
        if (value.isPressed) OnWeaponDirectSelect?.Invoke(1);
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            OnInteractInputReceived?.Invoke();
        }
    }
}
