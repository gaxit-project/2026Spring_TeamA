using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI rescueCountText;

    public void UpdateKillCountDisplay(int count)
    {
        if (killCountText != null)
        {
            killCountText.text = $"{count}";
        }
    }

    public void UpdateRescueCountDisplay(int count)
    {
        if (rescueCountText != null)
        {
            rescueCountText.text = $"{count}";
        }
    }
}
