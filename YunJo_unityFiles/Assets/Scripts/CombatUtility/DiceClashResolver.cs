using UnityEngine;

public static class DiceClashResolver
{
    public static DiceOutcome Resolve(PageDie a, PageDie b)
    {
        int rollA = a.Roll();
        int rollB = b.Roll();

        if (rollA > rollB)
            return DiceOutcome.Win;

        if (rollB > rollA)
            return DiceOutcome.Lose;

        return DiceOutcome.Draw;
    }
}