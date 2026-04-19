using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI reservedAmmoText;

    [SerializeField] private GameObject reloadBarRoot;
    [SerializeField] private Image reloadBarFill;

    private void Start()
    {
        SetReloadProgress(0f);
    }

    public void UpdateAmmoDisplay(int current, int stash)
    {
        currentAmmoText.text = current.ToString();
        reservedAmmoText.text = stash.ToString();
    }

    public void SetReloadProgress(float progress)
    {
        if (reloadBarRoot == null || reloadBarFill == null) return;

        reloadBarRoot.SetActive(progress > 0 && progress < 1f);
        reloadBarFill.fillAmount = progress;
    }
}
