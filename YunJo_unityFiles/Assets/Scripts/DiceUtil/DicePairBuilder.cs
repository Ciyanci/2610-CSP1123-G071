using UnityEngine;
using System.Collections.Generic;

public static class DicePairBuilder
{
    public static List<DicePair> Build(
        CombatPageRuntime a,
        CombatPageRuntime b)
    {
        List<DicePair> pairs = new();

        int count =
            Mathf.Min(a.dice.Count, b.dice.Count);

        // PAIR MATCHED DICE
        for (int i = 0; i < count; i++)
        {
            pairs.Add(new DicePair
            {
                a = a.dice[i],
                b = b.dice[i]
            });
        }

        // LEFTOVER A (unopposed)
        for (int i = count; i < a.dice.Count; i++)
        {
            pairs.Add(new DicePair
            {
                a = a.dice[i],
                b = null,
                outcome = DiceOutcome.Unopposed
            });
        }

        // LEFTOVER B (unopposed)
        for (int i = count; i < b.dice.Count; i++)
        {
            pairs.Add(new DicePair
            {
                a = null,
                b = b.dice[i],
                outcome = DiceOutcome.Unopposed
            });
        }

        return pairs;
    }
}