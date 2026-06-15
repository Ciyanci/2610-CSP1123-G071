using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TeamRosterSlot
{
    public UnitData unit;

    //null for leaders (uses unit.lockedKeypage instead)
    public KeypageData equippedKeypage;

    //player-configured deck for this unit this stage
    //it will start as a copy of unit.starterDeck, player edits here
    public List<CardData> configuredDeck = new();

    public bool IsEmpty => unit == null;

    public KeypageData GetEffectiveKeypage()
    {
        if (unit == null) return null;
        return unit.isLeader ? unit.lockedKeypage : equippedKeypage;
    }

    //call once when slot is first assigned a unit
    public void InitializeDeck()
    {
        configuredDeck = new List<CardData>(unit.starterDeck);

        if (unit.isLeader)
            foreach (var c in unit.uniqueCards)
                if (!configuredDeck.Contains(c))
                    configuredDeck.Add(c);
    }
}
