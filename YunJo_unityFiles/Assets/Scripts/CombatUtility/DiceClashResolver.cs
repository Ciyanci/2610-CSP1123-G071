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
        {
            b.destroyed = true;
            return DiceClashResult.Win;
        }

        if (rollB > rollA)
        {
            a.destroyed = true;
            return DiceClashResult.Lose;
        }

        return DiceClashResult.Draw;
    }
}