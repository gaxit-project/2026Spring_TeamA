using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "ScriptableObjects/ScoreData")]
public class ScoreData : ScriptableObject
{
    [Header("Zombie Kills Multiplier")]
    public int killMultiplier = 100;

    [Header("NPC Rescue Multiplier")]
    public int rescueMultiplier = 500;

    [Header("Vaccine Bonus")]
    public int vaccineBonus = 2000;

    [Header("Vaccine Penalty")]
    public int vaccinePenalty = -1000;
}
