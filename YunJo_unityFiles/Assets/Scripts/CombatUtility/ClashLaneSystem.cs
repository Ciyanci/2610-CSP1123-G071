using UnityEngine;
using System.Collections.Generic;

public class ClashLaneSystem
{
    public List<(CombatAction a, CombatAction b)> lanes = new();

    public void Build(List<CombatAction> actions)
    {
        lanes.Clear();

        for (int i = 0; i < actions.Count; i++)
        {
            for (int j = i + 1; j < actions.Count; j++)
            {
                var a = actions[i];
                var b = actions[j];

                if (a.user == b.target && b.user == a.target)
                    lanes.Add((a, b));
            }
        }
    }
}