using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/TeamRoster")]
public class TeamRoster : ScriptableObject
{
    [Header("Leader — locked, set per chapter")]
    public TeamRosterSlot leaderSlot = new();

    [Header("Assistants — player fills these")]
    public TeamRosterSlot[] assistantSlots = new TeamRosterSlot[3];

    void OnEnable()
    {
        for (int i = 0; i < assistantSlots.Length; i++)
            if (assistantSlots[i] == null)
                assistantSlots[i] = new TeamRosterSlot();
    }

    public List<TeamRosterSlot> GetFilledSlots()
    {
        var slots = new List<TeamRosterSlot>();

        if (!leaderSlot.IsEmpty)
            slots.Add(leaderSlot);

        foreach (var s in assistantSlots)
            if (s != null && !s.IsEmpty)
                slots.Add(s);

        return slots;
    }

    public bool IsValid()
    {
        return !leaderSlot.IsEmpty;
    }
}
