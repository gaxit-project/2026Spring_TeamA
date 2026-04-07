using TMPro;
using UnityEngine;

public class AmmoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI reservedAmmoText;

    public void UpdateAmmoDisplay(int current, int stash)
    {
        currentAmmoText.text = current.ToString();
        reservedAmmoText.text = stash.ToString();
    }
}
