using UnityEngine;
using System.Collections.Generic;

public static class IntentBuilder
{
    public static List<CombatIntent> Build(List<PreviewIntent> previews)
    {
        List<CombatIntent> intents = new();

        foreach (var p in previews)
        {
            if (p.user == null || p.target == null)
                continue;

            SpeedDie die = p.user.GetHighestAvailableDie();
            if (die == null)
                continue;

            die.used = true;

            intents.Add(new CombatIntent
            {
                user = p.user,
                target = p.target,
                card = p.card,
                speedDie = die,
                resolved = false,
                priority = die.value   // 🔥 FIX: define priority properly
            });
        }

        intents.Sort((a, b) => b.priority.CompareTo(a.priority));

        Debug.Log($"[INTENT BUILDER] Built {intents.Count} intents");
        return intents;
    }
}