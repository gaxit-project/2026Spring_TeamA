using UnityEngine;
using TMPro;

public class HPView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;

    public void UpdateHpDiaplay(int currrentHP)
    {
        if (hpText != null)
        {
            hpText.text = currrentHP.ToString();
        }
    }
}
