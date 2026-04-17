using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCountText;

    public void UpdateKillCountDisplay(int count)
    {
        if (killCountText != null)
        {
            killCountText.text = $"{count}";
        }
    }
}
