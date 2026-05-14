using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    [SerializeField] private TextData japaneseText;
    [SerializeField] private TextData englishText;

    public TextData CurrentTextData { get; private set; }

    /// <summary>
    /// 現在の言語が日本語かどうかを取得するプロパティ
    /// </summary>
    public bool IsJapanese => CurrentTextData == japaneseText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetLanguage(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(bool isJapanese)
    {
        CurrentTextData = isJapanese ? japaneseText : englishText;
    }
}
