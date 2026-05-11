using UnityEngine;

public static class DiceClashResolver
{
    public static DiceOutcome Resolve(
        PageDie a,
        PageDie b)
    {
        int rollA = a.Roll();
        int rollB = b.Roll();

        Debug.Log($"[DICE CLASH] {rollA} vs {rollB}");

        if (rollA > rollB)
            return DiceOutcome.Win;

        if (rollB > rollA)
            return DiceOutcome.Lose;

        return DiceOutcome.Draw;
    }
}