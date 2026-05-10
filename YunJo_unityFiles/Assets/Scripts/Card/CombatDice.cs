using UnityEngine;

public class CombatDice
{
    public DiceData data;
    public int currentRoll;

    public CombatDice(DiceData d)
    {
        data = d;
    }

    public int Roll()
    {
        currentRoll = Random.Range(data.minRoll, data.maxRoll + 1);
        return currentRoll;
    }
}