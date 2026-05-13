using UnityEngine;
using UnityEngine.UI;

public class SettingsSlidersManager : MonoBehaviour
{
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SESlider;

    public Slider GetBGMSlider()
    {
        return BGMSlider;
    }

    public Slider GetSESlider()
    {
        return SESlider;
    }
}
