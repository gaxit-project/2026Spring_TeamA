using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextData")]
public class TextData : ScriptableObject
{
    public string gameStartMission = "Escape the lab within 3 minutes!";
    public float gameStartDisplayTime = 4.0f;

    public string interactPrompt = "[X] : Talk";
    public string rescueSuccessMessage = "Success!";
    public float rescueMessageDisplayTime = 3.0f;

    public string npcDeathMessage = "Casualties among survivors";
    public float npcDeathDisplayTime = 3.0f;
}