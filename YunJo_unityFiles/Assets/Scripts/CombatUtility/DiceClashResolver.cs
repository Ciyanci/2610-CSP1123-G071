using UnityEngine;

public static class DiceClashResolver
{
    public static DiceClashResult Resolve(
        CombatDiceRuntime a,
        CombatDiceRuntime b)
    {
        int rollA = a.Roll();
        int rollB = b.Roll();

        Debug.Log($"[DICE CLASH] {rollA} vs {rollB}");

        if (rollA > rollB)
            return DiceClashResult.Win;

        if (rollB > rollA)
            return DiceClashResult.Lose;

        return DiceClashResult.Draw;
    }
}