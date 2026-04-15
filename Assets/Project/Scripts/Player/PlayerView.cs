using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerView : MonoBehaviour
{
    // EnemyDataを受け取るためのアクション
    public System.Action<EnemyData> OnHitByEnemy;

    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int AimPitchHash = Animator.StringToHash("AimPitch");
    private static readonly int FireTrigger = Animator.StringToHash("OnFire");
    private static readonly int ReloadTrigger = Animator.StringToHash("OnReload");

    public void Move(Vector3 move)
    {
        transform.Translate(move, Space.Self);

        if (animator != null)
        {
            bool moving = move.magnitude > 0.001f;
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
        //Debug.Log(pitch);
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

    // アニメーションに発射のタイミングを組み込む関数
    public void OnShoot()
    {
        OnFireEffectTiming?.Invoke();
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
    public System.Action<int> OnWeaponSwitchInputRecieved;
    public System.Action<int> OnWeaponDirectSelect;
    public System.Action<Collider> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
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
        Debug.Log($"[Input] Fire Pressed: {value.isPressed}");
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
}
