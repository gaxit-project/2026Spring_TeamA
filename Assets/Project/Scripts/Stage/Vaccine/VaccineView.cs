using UnityEngine;

/// <summary>
/// ワクチンの種類
/// </summary>
public enum VaccineType { A, B, C }

/// <summary>
/// ワクチンのView。取得されるまでエフェクトを流し続けます。
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class VaccineView : MonoBehaviour, IInteractable
{
    [SerializeField] private VaccineType type;
    [SerializeField] private GameObject glowEffect;

    public System.Action OnInteracted;

    public bool CanInteract { get; private set; } = true;

    private void Awake()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
    }

    /// <summary>
    /// 開始時にエフェクトを再生開始する
    /// </summary>
    private void Start()
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(true);
            var ps = glowEffect.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
        }
    }

    /// <summary>
    /// インタラクト時の処理
    /// </summary>
    public void Interact(GameObject interactor)
    {
        if (!CanInteract) return;
        CanInteract = false;

        // エフェクトを消す
        if (glowEffect != null) glowEffect.SetActive(false);

        // 本体を消す
        gameObject.SetActive(false);

        OnInteracted?.Invoke();
    }

    /// <summary>
    /// SOからテキストを取得
    /// </summary>
    public string GetInteractPrompt()
    {
        var data = UIManager.Instance.textData;
        if (data == null) return "";

        return type switch
        {
            VaccineType.A => data.vaccineAPrompt,
            VaccineType.B => data.vaccineBPrompt,
            VaccineType.C => data.vaccineCPrompt,
            _ => ""
        };
    }
}
