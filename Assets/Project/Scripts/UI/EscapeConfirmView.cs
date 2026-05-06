using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscapeConfirmView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private GameObject panel;

    [SerializeField] private GameObject firstSelectedButton; // 表示時に最初にフォーカスを当てるボタン

    [SerializeField] private GameObject[] uisToHide; // パネル表示中に消しておきたい他のUI

    private void Awake()
    {
        panel.SetActive(false);
    }


    private void OnEnable()
    {
        UIEvents.OnShowEscapeConfirm += ShowPanel;
        UIEvents.OnHideEscapeConfirm += HidePanel;
    }

    private void OnDestroy()
    {
        UIEvents.OnShowEscapeConfirm -= ShowPanel;
        UIEvents.OnHideEscapeConfirm -= HidePanel;
    }

    private void ShowPanel()
    {
        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (UIManager.Instance.textData != null)
        {
            warningText.text = UIManager.Instance.textData.vaccineWarningMessage;
        }

        // 邪魔なUIを一時的に隠す
        foreach (var ui in uisToHide)
        {
            if (ui != null) ui.SetActive(false);
        }

        // EventSystemで最初のボタンを選択状態にする
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
    private void HidePanel()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 隠していたUIを元に戻す
        foreach (var ui in uisToHide)
        {
            if (ui != null) ui.SetActive(true);
        }
        
        // 戻った場合は操作ブロックを解除
        PlayerPresenter.Instance.SetInputBlocked(false);
    }

    /// <summary>
    /// 「脱出する(Yes)」ボタンから呼ぶ
    /// </summary>
    public void OnClickEscape()
    {
        panel.SetActive(false);
        GamePresenter.Instance.TriggerGameClear();
    }
    /// <summary>
    /// 「戻る(No)」ボタンから呼ぶ
    /// </summary>
    public void OnClickCancel()
    {
        UIEvents.OnHideEscapeConfirm?.Invoke();
    }
}
