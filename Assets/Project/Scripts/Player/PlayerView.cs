using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    public void Move(Vector3 move)
    {
        transform.Translate(move, Space.Self);

        if (animator != null)
        {
            bool moving = move.magnitude > 0.001f;
            animator.SetBool(IsMoving, moving);
        }
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
            //animator.SetFloat("AimAngle", pitch);
        }
    }

    // 入力があったことをPresenterに知らせるためのイベント
    public System.Action<Vector2> OnMoveInputReceived;
    public System.Action<Vector2> OnLookInputReceived;

    public System.Action OnFireInputReceived;
    public System.Action OnReloadInputReceived;

    private void OnMove(InputValue value)
    {
        // 入力値を読み取って、イベントを購読している先に通知する
        OnMoveInputReceived?.Invoke(value.Get<Vector2>());
    }

    private void OnLook(InputValue value)
    {
        OnLookInputReceived?.Invoke(value.Get<Vector2>());
    }

    private void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            OnFireInputReceived?.Invoke();
        }
    }

    private void OnReload(InputValue value)
    {
        OnReloadInputReceived?.Invoke();
    }
}
