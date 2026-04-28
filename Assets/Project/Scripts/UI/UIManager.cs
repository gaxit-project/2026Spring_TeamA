using DG.Tweening;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public HPView hpview;
    public AmmoView ammoView;
    public WeaponHUD weaponHUD;

    public TextData textData;
    [SerializeField] private TextMeshProUGUI systemMessageText;
    [SerializeField] private TextMeshProUGUI interactPromptText;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (systemMessageText != null)
        {
            systemMessageText.color = new Color(systemMessageText.color.r, systemMessageText.color.g, systemMessageText.color.b, 0f);
        }

        if (interactPromptText != null) interactPromptText.gameObject.SetActive(false);
    }

    /// <summary>
    /// オブジェクトが有効になった時、UIEventsのイベントを購読する
    /// </summary>
    private void OnEnable()
    {
        UIEvents.OnShowMissionStartMessage += ShowMissionStartMessage;
        UIEvents.OnShowInteractPrompt += ShowInteractPrompt;
        UIEvents.OnHideInteractPrompt += HideInteractPrompt;
        UIEvents.OnShowSystemMessage += ShowSystemMessage;
        UIEvents.OnShowNpcDeathMessage += ShowNpcDeathMessage;
    }

    /// <summary>
    /// オブジェクトが無効になった時、エラーを防ぐために受信設定を解除する
    /// </summary>
    private void OnDisable()
    {
        UIEvents.OnShowMissionStartMessage -= ShowMissionStartMessage;
        UIEvents.OnShowInteractPrompt -= ShowInteractPrompt;
        UIEvents.OnHideInteractPrompt -= HideInteractPrompt;
        UIEvents.OnShowSystemMessage -= ShowSystemMessage;
        UIEvents.OnShowNpcDeathMessage -= ShowNpcDeathMessage;
    }

    private void ShowMissionStartMessage()
    {
        if (textData == null) return;
        ShowSystemMessage(textData.gameStartMission, textData.gameStartDisplayTime);
    }

    /// <summary>
    /// NPC死亡時のメッセージを表示する
    /// </summary>
    public void ShowNpcDeathMessage()
    {
        if (textData == null) return;
        ShowSystemMessage(textData.npcDeathMessage, textData.npcMessageDisplayTime);
    }

    /// <summary>
    /// 画面中央にシステムメッセージを表示し、フェードアウトさせる
    /// </summary>
    public void ShowSystemMessage(string text, float displayTime)
    {
        if (systemMessageText == null) return;
        systemMessageText.DOKill();
        systemMessageText.text = text;
        
        // 表示アニメーション
        systemMessageText.color = new Color(systemMessageText.color.r, systemMessageText.color.g, systemMessageText.color.b, 1f);
        systemMessageText.DOFade(0f, 1f).SetDelay(displayTime);
    }

    /// <summary>
    /// インタラクトの説明テキストを表示する
    /// </summary>
    public void ShowInteractPrompt(string text)
    {
        if (interactPromptText == null) return;
        interactPromptText.text = text;
        interactPromptText.gameObject.SetActive(true);
    }

    /// <summary>
    /// インタラクトの説明テキストを隠す
    /// </summary>
    public void HideInteractPrompt()
    {
        if (interactPromptText == null) return;
        interactPromptText.gameObject.SetActive(false);
    }
}
