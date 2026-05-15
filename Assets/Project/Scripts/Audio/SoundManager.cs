using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    //音源
    [SerializeField] private AudioClip[] seList;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip clearBGM;
    [SerializeField] private AudioClip gameOverBGM;

    //鳴らす音
    [SerializeField] private AudioSource audioSourceSE;
    [SerializeField] private AudioSource audioSourceBGM;

    [Header("スライダー")]
    [SerializeField] private Slider SESlider;
    [SerializeField] private Slider BGMSlider;

    private float _seVolume = 0.5f;
    private float _bgmVolume = 0.5f;

    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null) return null;
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            SceneManager.activeSceneChanged -= CallGetSliders;
        }
    }

    void Start()
    {
        //シーン切り替え時に関数を呼び出すようにする
        SceneManager.activeSceneChanged += CallGetSliders;

        // 起動時にタイトル画面ならBGM再生
        if (SceneManager.GetActiveScene().name == "Title")
        {
            PlayTitleBGM();
        }

        //AudioSourceが無い場合はアタッチする
        if (audioSourceSE == null)
        {
            audioSourceSE = gameObject.AddComponent<AudioSource>();
        }
        if (audioSourceBGM == null)
        {
            audioSourceBGM = gameObject.AddComponent<AudioSource>();
        }

        //保存した音量のデータをロード
        LoadVolumeSetting();

        GetSliders();
    }

    /// <summary>
    /// GetSlidersを呼び出す
    /// </summary>
    /// <param name="a">ダミー</param>
    /// <param name="b">ダミー</param>
    private void CallGetSliders(Scene a, Scene b)
    {
        GetSliders();

        if (b.name == "Title")
        {
            PlayTitleBGM();
        }
        else if (b.name == "MainScene" || b.name == "Stage1") // メインシーンの名前を追加
        {
            PlayMainBGM();
        }
    }

    /// <summary>
    /// スライダーを取得して初期化する
    /// </summary>
    private void GetSliders()
    {
        //初期化
        SESlider = null;
        BGMSlider = null;

        //シーン内にSettingsSlidersManagerがあればスライダーを取得
        if (GameObject.FindFirstObjectByType<SettingsSlidersManager>() is SettingsSlidersManager SSM)
        {
            SESlider = SSM.GetSESlider();
            BGMSlider = SSM.GetBGMSlider();
        }

        if (SESlider != null && BGMSlider != null)
        {
            InitializeSliders();
        }
        else
        {
            Debug.LogWarning("SettingsSlidersManagerが見つかりませんでした");
        }
    }

    private void LoadVolumeSetting()
    {
        if (PlayerPrefs.HasKey("SEVolume") && PlayerPrefs.HasKey("BGMVolume"))
        {
            _seVolume = PlayerPrefs.GetFloat("SEVolume");
            _bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        }

        audioSourceSE.volume = _seVolume;
        audioSourceBGM.volume = _bgmVolume;
    }

    /// <summary>
    /// スライダー初期化
    /// </summary>
    public void InitializeSliders()
    {

        //スライダー初期値を反映
        SESlider.value = _seVolume;
        BGMSlider.value = _bgmVolume;


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
        _seVolume = SESlider.value;
        audioSourceSE.volume = SESlider.value;
        PlayerPrefs.SetFloat("SEVolume", _seVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// BGMの値が変更されたときの処理
    /// </summary>
    public void OnBGMVolumeChange()
    {
        _bgmVolume = BGMSlider.value;
        audioSourceBGM.volume = BGMSlider.value;
        PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
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
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        // 既に同じ曲が流れているなら何もしない
        if (audioSourceBGM.clip == clip && audioSourceBGM.isPlaying)
        {
            return;
        }

        audioSourceBGM.clip = clip;
        audioSourceBGM.Play();
    }

    public void PlayTitleBGM() => PlayBGM(titleBGM);
    public void PlayMainBGM() => PlayBGM(mainBGM);
    public void PlayClearBGM() => PlayBGM(clearBGM);
    public void PlayGameOverBGM() => PlayBGM(gameOverBGM);

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    /// <summary>
    /// 音量を保存する
    /// </summary>
    private void SaveVolumeSetting()
    {
        PlayerPrefs.SetFloat("SEVolume", _seVolume);
        PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ゲーム終了時に音量保存
    /// </summary>
    private void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= CallGetSliders;
        SaveVolumeSetting();
    }
}