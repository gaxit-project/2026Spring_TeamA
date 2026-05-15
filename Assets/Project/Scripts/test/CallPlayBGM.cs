using UnityEngine;

public class CallPlayBGM : MonoBehaviour
{
    [SerializeField] private int index = 0;

    void Start()
    {
        //SoundManager.Instance.PlayBGM(index);
    }

    private void OnDestroy()
    {
        SoundManager.Instance?.StopBGM();
    }
}
