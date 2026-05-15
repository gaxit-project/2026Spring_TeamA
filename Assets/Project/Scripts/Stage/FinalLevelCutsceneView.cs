using UnityEngine;
using TMPro;

/// <summary>
/// 最終階層でのカメラ演出およびテキスト表示を管理するView
/// </summary>
public class FinalLevelCutsceneView : MonoBehaviour
{
    [SerializeField, Tooltip("ワクチンを映すカメラ")] 
    private GameObject[] _vaccineCameras;
    
    [SerializeField, Tooltip("出口を映すカメラ")] 
    private GameObject _exitCamera;
    
    [SerializeField, Tooltip("目標を表示するテキスト")] 
    private TextMeshProUGUI _objectiveText;

    [SerializeField, Tooltip("1つのカメラを表示する秒数")] 
    private float _cameraDuration = 2.0f;

    [SerializeField, Tooltip("演出中に隠しておきたい他のUI")] 
    private GameObject[] _uisToHide;

    public float CameraDuration => _cameraDuration;
    public int VaccineCameraCount => _vaccineCameras != null ? _vaccineCameras.Length : 0;

    /// <summary>
    /// 演出中に他のUIを隠す
    /// </summary>
    public void HideUI()
    {
        if (_uisToHide == null) return;
        foreach (var ui in _uisToHide)
        {
            if (ui != null) ui.SetActive(false);
        }
    }

    /// <summary>
    /// 演出終了後に他のUIを再表示する
    /// </summary>
    public void ShowUI()
    {
        if (_uisToHide == null) return;
        foreach (var ui in _uisToHide)
        {
            if (ui != null) ui.SetActive(true);
        }
    }

    /// <summary>
    /// 全ての演出用カメラを非アクティブにする
    /// </summary>
    public void HideAllCameras()
    {
        if (_vaccineCameras != null)
        {
            foreach (var cam in _vaccineCameras)
            {
                if (cam != null) cam.SetActive(false);
            }
        }
        if (_exitCamera != null) _exitCamera.SetActive(false);
    }

    /// <summary>
    /// 指定されたインデックスのワクチンカメラとテキストを表示する
    /// </summary>
    public void ShowVaccineCamera(int index)
    {
        HideAllCameras();
        if (_vaccineCameras != null && index >= 0 && index < _vaccineCameras.Length && _vaccineCameras[index] != null)
        {
            _vaccineCameras[index].SetActive(true);
        }
        
        if (_objectiveText != null && LanguageManager.Instance?.CurrentTextData != null)
        {
            // オブジェクトを有効化し、透明度を1に設定して見えるようにする
            _objectiveText.gameObject.SetActive(true);
            Color c = _objectiveText.color;
            c.a = 1f;
            _objectiveText.color = c;

            _objectiveText.text = LanguageManager.Instance.CurrentTextData.finalLevelVaccineObjective;
        }
    }

    /// <summary>
    /// 出口カメラとテキストを表示する
    /// </summary>
    public void ShowExitCamera()
    {
        HideAllCameras();
        if (_exitCamera != null)
        {
            _exitCamera.SetActive(true);
        }

        if (_objectiveText != null && LanguageManager.Instance?.CurrentTextData != null)
        {
            // オブジェクトを有効化し、透明度を1に設定して見えるようにする
            _objectiveText.gameObject.SetActive(true);
            Color c = _objectiveText.color;
            c.a = 1f;
            _objectiveText.color = c;

            _objectiveText.text = LanguageManager.Instance.CurrentTextData.finalLevelExitObjective;
        }
    }

    /// <summary>
    /// テキストを非表示にする
    /// </summary>
    public void HideText()
    {
        if (_objectiveText != null)
        {
            _objectiveText.text = string.Empty;
        }
    }
}
