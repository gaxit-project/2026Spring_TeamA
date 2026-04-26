using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    //音源
    [SerializeField] private AudioClip[] seList;
    [SerializeField] private AudioClip[] bgmList;

    //鳴らす音
    [SerializeField] private AudioSource audioSourceSE;
    [SerializeField] private AudioSource audioSourceBGM;

    [Header("スライダー")]
    [SerializeField] private Slider SESlider;
    [SerializeField] private Slider BGMSlider;

    private float seVolume = 0.5f;
    private float bgmVolume = 0.5f;

    public static SoundManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            //インスタンスがなければ設定
            Instance = this;
        }
        else
        {
            //既にあれば破棄
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        //シーン切り替え時に関数を呼び出すようにする
        SceneManager.activeSceneChanged += GetSliders;


        //保存した音量のデータをロード
        LoadVolumeSetting();

        if (SESlider == null || BGMSlider == null)
        {
            SESlider = GameObject.Find("SESlider").GetComponent<Slider>();
            BGMSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();
            GameObject.Find("SettingMenu").SetActive(false);
        }
        if (SESlider != null && BGMSlider != null)
        {
            InitializeSliders();
        }
        if (audioSourceSE == null)
        {
            audioSourceSE = gameObject.AddComponent<AudioSource>();
        }
        if (audioSourceBGM == null)
        {
            audioSourceBGM = gameObject.AddComponent<AudioSource>();
            PlayBGM(0);
        }
    }

    /// <summary>
    /// タイトルシーンのときスライダーを取得して初期化する
    /// </summary>
    /// <param name="a">ダミー</param>
    /// <param name="b">ダミー</param>
    private void GetSliders(Scene a, Scene b)
    {
        //初期化
        SESlider = null;
        BGMSlider = null;

        //スライダーと設定画面があれば取得
        SESlider = GameObject.Find("SESlider")?.GetComponent<Slider>();
        BGMSlider = GameObject.Find("BGMSlider")?.GetComponent<Slider>();
        GameObject.Find("SettingMenu")?.SetActive(false);

        if (SESlider != null && BGMSlider != null)
        {
            InitializeSliders();
        }
        else
        {
            Debug.LogWarning("音量設定スライダーが見つかりませんでした");
        }
    }

    private void LoadVolumeSetting()
    {
        if (PlayerPrefs.HasKey("SEVolume") && PlayerPrefs.HasKey("BGMVolume"))
        {
            seVolume = PlayerPrefs.GetFloat("SEVolume");
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        }

        audioSourceSE.volume = seVolume;
        audioSourceBGM.volume = bgmVolume;
    }

    /// <summary>
    /// スライダー初期化
    /// </summary>
    public void InitializeSliders()
    {

        //スライダー初期値を反映
        SESlider.value = seVolume;
        BGMSlider.value = bgmVolume;


        //イベントリスナーを登録
        SESlider.onValueChanged.RemoveAllListeners();
        SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });

        BGMSlider.onValueChanged.RemoveAllListeners();
        BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });


    }

    /// <summary>
    /// SEの値が変更されたときの処理
    /// </summary>
    public void OnSEVolumeChange()
    {
        seVolume = SESlider.value;
        audioSourceSE.volume = SESlider.value;
        PlayerPrefs.SetFloat("SEVolume", seVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// BGMの値が変更されたときの処理
    /// </summary>
    public void OnBGMVolumeChange()
    {
        bgmVolume = BGMSlider.value;
        audioSourceBGM.volume = BGMSlider.value;
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="index">再生したいSEList番号</param>
    public void PlaySound(int index)
    {
        if (seList.Length - 1 < index)
        {
            Debug.LogError("seList[" + index + "]の音声データは存在しません！");
            return;
        }
        audioSourceSE.clip = seList[index];
        audioSourceSE.PlayOneShot(seList[index]);
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="index">再生したいBGMList番号</param>
    public void PlayBGM(int index)
    {
        if (bgmList.Length - 1 < index)
        {
            Debug.LogError("bgmList[" + index + "]の音声データは存在しません！");
            return;
        }
        audioSourceBGM.clip = bgmList[index];
        audioSourceBGM.Play();
    }

    /// <summary>
    /// 音量を保存する
    /// </summary>
    private void SaveVolumeSetting()
    {
        PlayerPrefs.SetFloat("SEVolume", seVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ゲーム終了時に音量保存
    /// </summary>
    private void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= GetSliders;
        SaveVolumeSetting();
    }
}