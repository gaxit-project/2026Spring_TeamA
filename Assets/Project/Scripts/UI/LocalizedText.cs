using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    public enum TextKey
    {
        GameStartMission,
        InteractPrompt,
        StopPanickingPrompt,
        NpcInitialRescueMessage,
        NpcPanicMessage,
        NpcRescueAfterEscapeMessage,
        NpcDeathMessage,
        VaccineAPrompt,
        VaccineBPrompt,
        VaccineCPrompt,
        GoalPrompt,
        VaccineWarningMessage,
        GameClearMessage,
        LoadingMessage,
        ResultZombieKillsLabel,
        ResultNpcRescuedLabel,
        ResultVaccineBonusLabel,
        ResultVaccinePenaltyLabel,
        ResultTotalScoreLabel,
        ResultTotalLabel,
        ResultRankLabel
    }

    [SerializeField] private TextKey textKey;

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// 言語設定に基づいてテキスト表示を更新する
    /// </summary>
    public void UpdateText()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
        if (textComponent == null) return;

        if (LanguageManager.Instance == null || LanguageManager.Instance.CurrentTextData == null)
        {
            return;
        }

        TextData data = LanguageManager.Instance.CurrentTextData;
        textComponent.text = GetTextValue(data, textKey);
    }

    /// <summary>
    /// Keyに対応するTextDataの文字列を取得する
    /// </summary>
    private string GetTextValue(TextData data, TextKey key)
    {
        switch (key)
        {
            case TextKey.GameStartMission: return data.gameStartMission;
            case TextKey.InteractPrompt: return data.interactPrompt;
            case TextKey.StopPanickingPrompt: return data.stopPanickingPrompt;
            case TextKey.NpcInitialRescueMessage: return data.npcInitialRescueMessage;
            case TextKey.NpcPanicMessage: return data.npcPanicMessage;
            case TextKey.NpcRescueAfterEscapeMessage: return data.npcRescueAfterEscapeMessage;
            case TextKey.NpcDeathMessage: return data.npcDeathMessage;
            case TextKey.VaccineAPrompt: return data.vaccineAPrompt;
            case TextKey.VaccineBPrompt: return data.vaccineBPrompt;
            case TextKey.VaccineCPrompt: return data.vaccineCPrompt;
            case TextKey.GoalPrompt: return data.goalPrompt;
            case TextKey.VaccineWarningMessage: return data.vaccineWarningMessage;
            case TextKey.GameClearMessage: return data.gameClearMessage;
            case TextKey.LoadingMessage: return data.loadingMessage;
            case TextKey.ResultZombieKillsLabel: return data.resultZombieKillsLabel;
            case TextKey.ResultNpcRescuedLabel: return data.resultNpcRescuedLabel;
            case TextKey.ResultVaccineBonusLabel: return data.resultVaccineBonusLabel;
            case TextKey.ResultVaccinePenaltyLabel: return data.resultVaccinePenaltyLabel;
            case TextKey.ResultTotalScoreLabel: return data.resultTotalScoreLabel;
            case TextKey.ResultTotalLabel: return data.resultTotalLabel;
            case TextKey.ResultRankLabel: return data.resultRankLabel;
            default: return string.Empty;
        }
    }
}
