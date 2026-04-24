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

    public void ShowMissionStartMessage()
    {
        ShowSystemMessage(textData.gameStartMission, textData.gameStartDisplayTime);
    }

    public void ShowRescueMessage()
    {
        ShowSystemMessage(textData.rescueSuccessMessage, textData.rescueMessageDisplayTime);
    }

    public void ShowNpcDeathMessage()
    {
        if (textData == null) return;
        ShowSystemMessage(textData.npcDeathMessage, textData.npcDeathDisplayTime);
    }

    public void ShowSystemMessage(string text, float displayTime)
    {
        if (systemMessageText == null) return;
        systemMessageText.DOKill();
        systemMessageText.text = text;
        
        // 表示アニメーション
        systemMessageText.color = new Color(systemMessageText.color.r, systemMessageText.color.g, systemMessageText.color.b, 1f);
        systemMessageText.DOFade(0f, 1f).SetDelay(displayTime);
    }

    public void ShowInteractPrompt(string text)
    {
        if (interactPromptText == null) return;
        interactPromptText.text = text;
        interactPromptText.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        if (interactPromptText == null) return;
        interactPromptText.gameObject.SetActive(false);
    }
}
