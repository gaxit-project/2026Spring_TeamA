using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();

            audioSource.playOnAwake = true;
            audioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// BGMを変更
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
