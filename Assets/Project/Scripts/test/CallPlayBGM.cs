using UnityEngine;

public class CallPlayBGM : MonoBehaviour
{
    [SerializeField] private int index = 0;

    void Start()
    {
        SoundManager.Instance.PlayBGM(index);
    }
}
