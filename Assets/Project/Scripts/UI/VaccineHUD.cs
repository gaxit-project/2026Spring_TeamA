using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ワクチンの回収状態を画面右上のUIに反映するクラス
/// </summary>
public class VaccineHUD : MonoBehaviour
{
    [Header("ワクチンA, B, Cの画像コンポーネント")]
    [SerializeField] private Image imageA;
    [SerializeField] private Image imageB;
    [SerializeField] private Image imageC;

    [Header("表示カラー設定")]
    [SerializeField] private Color uncollectedColor = new Color(1, 1, 1, 0.2f); // 未回収（半透明）
    [SerializeField] private Color collectedColor = Color.white;                // 回収済（不透明/色付き）

    [Header("表示階層の設定")]
    [SerializeField] private GameObject hudRoot;                               // 表示切り替えするルートオブジェクト
    [SerializeField] private int targetFloor = 3;                              // 表示させたい最後の階層

    private void Awake()
    {
        ResetHUD();
    }

    private void Start()
    {
        // 初期階層に基づいて表示状態を判定
        if (PlayerPresenter.Instance != null)
        {
            UpdateVisibility(PlayerPresenter.Instance.CurrentFloor);
        }
        else
        {
            UpdateVisibility(1);
        }
    }

    private void OnEnable()
    {
        UIEvents.OnVaccineCollected += SetVaccineCollected;
        UIEvents.OnFloorChanged += UpdateVisibility;
    }

    private void OnDisable()
    {
        UIEvents.OnVaccineCollected -= SetVaccineCollected;
        UIEvents.OnFloorChanged -= UpdateVisibility;
    }

    /// <summary>
    /// 現在の階層が目標階層と一致する場合のみUIを表示する
    /// </summary>
    /// <param name="currentFloor">現在の階層番号</param>
    private void UpdateVisibility(int currentFloor)
    {
        if (hudRoot != null)
        {
            hudRoot.SetActive(currentFloor == targetFloor);
        }
    }

    /// <summary>
    /// HUDの表示を未回収状態（初期状態）にリセットする
    /// </summary>
    public void ResetHUD()
    {
        if (imageA != null) imageA.color = uncollectedColor;
        if (imageB != null) imageB.color = uncollectedColor;
        if (imageC != null) imageC.color = uncollectedColor;
    }

    /// <summary>
    /// 回収されたワクチンの種類に応じて、該当のUIを点灯させる
    /// </summary>
    /// <param name="type">回収されたワクチンの種類</param>
    public void SetVaccineCollected(VaccineType type)
    {
        switch (type)
        {
            case VaccineType.A:
                if (imageA != null) imageA.color = collectedColor;
                break;
            case VaccineType.B:
                if (imageB != null) imageB.color = collectedColor;
                break;
            case VaccineType.C:
                if (imageC != null) imageC.color = collectedColor;
                break;
        }
    }
}
