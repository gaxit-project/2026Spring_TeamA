using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HPView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image hpGaugeImage;
    [SerializeField] private PlayerData playerData;


    [SerializeField] private Color safeColor = Color.green;     // 安全
    [SerializeField] private Color warningColor = Color.yellow; // 注意
    [SerializeField] private Color dangerColor = Color.red;     // 瀕死
    
    private float maxHP;

    public void UpdateHpDiaplay(int currentHP)
    {
        if (hpText != null)
        {
            //hpText.text = currentHP.ToString();
        }

        if (hpGaugeImage != null && playerData != null)
        {
            float maxHP = playerData.hp;
            float hpPercentage = currentHP / maxHP;

            hpGaugeImage.DOFillAmount(hpPercentage, 0.3f).SetEase(Ease.OutCubic);
            if (hpPercentage > 0.5f)
            {
                hpGaugeImage.DOColor(safeColor, 0.1f);
            }
            else if (hpPercentage > 0.2f)
            {
                hpGaugeImage.DOColor(warningColor, 0.1f);
            }
            else
            {
                hpGaugeImage.DOColor(dangerColor, 0.1f);
            }
        }
    }
}
