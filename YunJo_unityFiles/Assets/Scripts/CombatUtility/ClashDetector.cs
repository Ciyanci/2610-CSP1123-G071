using System.Collections.Generic;
using UnityEngine;

public static class ClashDetector
{
    public static List<ClashPair> Build(List<CombatIntent> intents)
    {
        List<ClashPair> clashes = new();
        HashSet<CombatIntent> used = new();

        for (int i = 0; i < intents.Count; i++)
        {
            var a = intents[i];

            if (used.Contains(a))
                continue;

            for (int j = i + 1; j < intents.Count; j++)
            {
                var b = intents[j];

                if (used.Contains(b))
                    continue;

                bool isClash =
                    a.user == b.target &&
                    a.target == b.user;

                if (!isClash)
                    continue;

                clashes.Add(new ClashPair(a, b));

                used.Add(a);
                used.Add(b);

                break;
            }
        }

        return clashes;
    }
}