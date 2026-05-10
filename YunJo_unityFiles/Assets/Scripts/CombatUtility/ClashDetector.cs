using UnityEngine;
using System.Collections.Generic;

public static class ClashDetector
{
    public static List<ClashPair> Build(
        List<CombatIntent> intents)
    {
        List<ClashPair> clashes = new();

        HashSet<CombatIntent> used = new();

        for (int i = 0; i < intents.Count; i++)
        {
            CombatIntent a = intents[i];

            if (used.Contains(a))
                continue;

            for (int j = i + 1; j < intents.Count; j++)
            {
                CombatIntent b = intents[j];

                if (used.Contains(b))
                    continue;

                bool clash =
                    a.user == b.target &&
                    a.target == b.user;

                if (!clash)
                    continue;

                clashes.Add(new ClashPair(a, b));

                used.Add(a);
                used.Add(b);

                Debug.Log($"[CLASH DETECTED] {a.user.name} <-> {b.user.name}");

                break;
            }
        }

        return clashes;
    }
}