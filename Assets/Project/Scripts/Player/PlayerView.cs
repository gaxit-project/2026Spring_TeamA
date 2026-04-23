using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerView : MonoBehaviour
{
    // EnemyDataを受け取るためのアクション
    public System.Action<EnemyData> OnHitByEnemy;

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;

    private Rigidbody _rb;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int AimPitchHash = Animator.StringToHash("AimPitch");
    private static readonly int FireTrigger = Animator.StringToHash("OnFire");
    private static readonly int ReloadTrigger = Animator.StringToHash("OnReload");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        if (_rb != null)
        {
            Vector3 worldVel = transform.TransformDirection(velocity);

            _rb.linearVelocity = new Vector3(worldVel.x, _rb.linearVelocity.y, worldVel.z);
        }

        if (animator != null)
        {
            bool moving = velocity.magnitude > 0.001f;
            animator.SetBool(IsMoving, moving);
        }
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

    private string targetTag;
    public void SetTargetTag(string tag) => targetTag = tag;

    // 入力があったことをPresenterに知らせるためのイベント
    public System.Action<Vector2> OnMoveInputReceived;
    public System.Action<Vector2> OnLookInputReceived;

    public System.Action<bool> OnDashInputReceived;
    public System.Action<bool> OnAimInputReceived;
    public System.Action<bool> OnFireInputReceived;
    
    public System.Action OnFireEffectTiming;
    public System.Action OnReloadInputReceived;
    public System.Action OnInteractInputReceived;
    
    public System.Action<int> OnWeaponSwitchInputRecieved;
    public System.Action<int> OnWeaponDirectSelect;
    

    public System.Action<Collider> OnTriggerEnterEvent;
    public System.Action<Collider> OnTriggerExitEvent;

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
        if (value.isPressed) OnInteractInputReceived?.Invoke();
    }
}
