using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextData")]
public class TextData : ScriptableObject
{
    public string gameStartMission = "Escape the lab within 3 minutes!"; // 初めの説明
    public float gameStartDisplayTime = 4.0f;

    public string interactPrompt = "[Y] : Talk";
    public string stopPanickingPrompt = "[Y] : Call";

    public string npcInitialRescueMessage = "Thank you found me!"; // NPCを助けた時
    public string npcPanicMessage = "No, stay away from me!"; // NPCがびっくりして逃げた時
    public string npcRescueAfterEscapeMessage = "Wait, you're human... I'm so sorry, I panicked!"; // 逃げた後に捕まえられた時
    public string npcDeathMessage = "Oh no! What have I done..."; // 誤射してNPCが死んだ時

    /// <summary>
    /// ワクチン収集用のテキスト
    /// </summary>
    public string vaccineAPrompt = "[Y] : Collect Vaccine A";
    public string vaccineBPrompt = "[Y] : Collect Vaccine B";
    public string vaccineCPrompt = "[Y] : Collect Vaccine C";

    /// <summary>
    /// ゴール関連のテキスト
    /// </summary>
    public string goalPrompt = "[Y] : Escape";
    [TextArea] public string vaccineWarningMessage = "You haven't collected all vaccines. Escape anyway?";

    public string gameClearMessage = "GAME CLEAR";
    public string loadingMessage = "Loading...";

    /// <summary>
    /// リザルト画面用のテキスト
    /// </summary>
    [Header("Result Screen Texts")]
    public string resultZombieKillsLabel = "Zombies Defeated";
    public string resultNpcRescuedLabel = "NPCs Rescued";
    public string resultVaccineBonusLabel = "Vaccine Bonus";
    public string resultVaccinePenaltyLabel = "Vaccine Penalty";
    public string resultTotalScoreLabel = "Total Score";

    public float npcMessageDisplayTime = 3.0f;
}