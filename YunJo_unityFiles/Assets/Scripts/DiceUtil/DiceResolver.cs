using UnityEngine;

public static class DiceResolver
{
    public static int Roll(CombatDice dice)
    {
        return Random.Range(
            dice.data.minRoll,
            dice.data.maxRoll + 1
        );
    }
}