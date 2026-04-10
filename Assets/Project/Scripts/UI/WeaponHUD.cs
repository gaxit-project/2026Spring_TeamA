using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    
    [SerializeField] private Image iconImage;

    private CancellationTokenSource fadeCts;

    public void UpdateWeaponUI(GunData data)
    {
        //iconImage.sprite = data.gunIcon;

        if (nameText != null)
        {
            nameText.text = data.gunName;
            FadeName().Forget();
        }
    }

    private async UniTaskVoid FadeName()
    {
        fadeCts?.Cancel();
        fadeCts = new CancellationTokenSource();

        nameText.alpha = 1f;
        await UniTask.Delay(2000, cancellationToken: fadeCts.Token);

        while (nameText.alpha > 0)
        {
            nameText.alpha -= Time.deltaTime * 2;
            await UniTask.Yield(fadeCts.Token);
        }
    }
}
