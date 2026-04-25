using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextData")]
public class TextData : ScriptableObject
{
    public string gameStartMission = "Escape the lab within 3 minutes!";
    public float gameStartDisplayTime = 4.0f;

    public string interactPrompt = "[Y] : Talk";
    public string stopPanickingPrompt = "[Y] : Call";

    public string npcInitialRescueMessage = "Thank you found me!";
    public string npcPanicMessage = "No, stay away from me!";
    public string npcRescueAfterEscapeMessage = "Wait, you're human... I'm so sorry, I panicked!";
    public string npcDeathMessage = "Oh no! What have I done...";

    public float npcMessageDisplayTime = 3.0f;
}