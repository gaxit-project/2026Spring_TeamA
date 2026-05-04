using UnityEngine;

/// <summary>
/// 個々のワクチンの挙動を管理するView
/// </summary>
public class VaccineItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string vaccineName; // 例：ワクチンA
    [SerializeField] private ParticleSystem glowParticle;

    public System.Action OnInteracted;

    public bool CanInteract { get; private set; } = true;

    /// <summary>
    /// プレイヤーがインタラクトした際の処理
    /// </summary>
    public void Interact(GameObject interactor)
    {
        if (!CanInteract) return;

        CanInteract = false;
        if (glowParticle != null) glowParticle.Stop();

        // 回収されたら非表示にする（または破棄）
        gameObject.SetActive(false);
        OnInteracted?.Invoke();
    }

    /// <summary>
    /// 画面に表示するインタラクト文言
    /// </summary>
    public string GetInteractPrompt() => $"ワクチン {vaccineName} を回収";
}
