using UnityEngine;

/// <summary>
/// ゴール地点のインタラクト管理
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class GoalPoint : MonoBehaviour, IInteractable
{
    public bool CanInteract { get; private set; } = true;


    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// ゴール時の処理（ワクチン収集状態による分岐）
    /// </summary>
    public void Interact(GameObject interactor)
    {
        if (!CanInteract) return;

        if (SessionData.IsVaccineCleared)
        {
            CanInteract = false;
            GamePresenter.Instance.TriggerGameClear();
        }
        else
        {
            // 集めていなければ確認UIを出す
            // UI操作中にプレイヤーが動けないようにブロックする
            PlayerPresenter.Instance.SetInputBlocked(true);

            UIEvents.OnShowEscapeConfirm?.Invoke();
        }
    }

    public string GetInteractPrompt()
    {
        var data = UIManager.Instance.textData;
        return data != null ? data.goalPrompt : "Escape";
    }
}
