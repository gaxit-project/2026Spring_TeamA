using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextData")]
public class TextData : ScriptableObject
{
        /// <summary>
    /// タイトル画面用のテキスト
    /// </summary>
    [Header("Title Screen Texts")]
    public string gameTitleName = "BIO ESCAPE";
    public string titleStartButton = "Start";
    public string titleSettingsButton = "Settings";
    public string titleExitButton = "Exit";
    public string titleReturnButton = "Return";
    public string titleAudioButton = "Audio";
    public string titleLanguageButton = "Language";
    public string titleCreditButton = "Credit";
    public string titleCreditText = "";

    /// <summary>
    /// ポーズ画面用のテキスト
    /// </summary>
    [Header("Pause Screen Texts")]
    public string pauseResumeButton = "Resume";
    public string pauseSettingsButton = "Settings";
    public string pauseQuitButton = "Quit";

    /// <summary>
    /// ゲームスタート時のテキスト
    /// </summary>
    [Header("Game Start Screen Texts")]
    public string gameStartMission = "Escape the lab within 3 minutes!"; // 初めの説明
    public float gameStartDisplayTime = 4.0f;

    /// <summary>
    /// インタラクト時のテキスト
    /// </summary>
    [Header("Interact Screen Texts")]
    public string interactPrompt = "[Y] : Talk";
    public string stopPanickingPrompt = "[Y] : Call";

    /// <summary>
    /// NPC関連のテキスト
    /// </summary>
    [Header("NPC Texts")]
    public string npcInitialRescueMessage = "Thank you found me!"; // NPCを助けた時
    public string npcPanicMessage = "No, stay away from me!"; // NPCがびっくりして逃げた時
    public string npcRescueAfterEscapeMessage = "Wait, you're human... I'm so sorry, I panicked!"; // 逃げた後に捕まえられた時
    public string npcDeathMessage = "Oh no! What have I done..."; // 誤射してNPCが死んだ時
    public float npcMessageDisplayTime = 3.0f;

    /// <summary>
    /// ワクチン収集用のテキスト
    /// </summary>
    public string vaccineAPrompt = "[Y] : Collect Vaccine A";
    public string vaccineBPrompt = "[Y] : Collect Vaccine B";
    public string vaccineCPrompt = "[Y] : Collect Vaccine C";

    /// <summary>
    /// ゴール関連のテキスト
    /// </summary>
    [Header("Goal Texts")]
    public string goalPrompt = "[Y] : Escape";
    [TextArea] public string vaccineWarningMessage = "You haven't collected all vaccines. Escape anyway?";
    public string confirmYesButton = "Yes";
    public string confirmNoButton = "No";

    /// <summary>
    /// ゲームクリア・ロード画面用のテキスト
    /// </summary>
    [Header("Game End Texts")]
    public string gameClearMessage = "GAME CLEAR";
    public string loadingMessage = "Loading...";

    /// <summary>
    /// リザルト画面用のテキスト
    /// </summary>
    [Header("Result Screen Texts")]
    public string resultSceneTitle = "Result";
    public string resultZombieKillsLabel = "Zombies Defeated";
    public string resultNpcRescuedLabel = "NPCs Rescued";
    public string resultVaccineBonusLabel = "Vaccine Bonus";
    public string resultVaccinePenaltyLabel = "Vaccine Penalty";
    public string resultTimeBonusLabel = "Time Bonus";
    public string resultTotalScoreLabel = "Total Score";
    public string resultRankLabel = "Rank";
    public string resultReturnToTitleButton = "Title";

    /// <summary>
    /// 最終階層カットシーン用のテキスト
    /// </summary>
    [Header("Final Level Cutscene Texts")]
    public string finalLevelVaccineObjective = "Collect 3 Vaccines";
    public string finalLevelExitObjective = "Escape!";
}