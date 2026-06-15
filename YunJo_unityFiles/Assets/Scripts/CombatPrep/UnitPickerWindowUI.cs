using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitPickerWindowUI : MonoBehaviour
{
    public Transform        container;
    public UnitPickerEntryUI entryPrefab;
    public Button           closeButton;

    List<UnitPickerEntryUI> spawned = new();
    int assistantIndex;

    void Awake()
    {
        closeButton?.onClick.AddListener(Close);
    }

    public void Open(int index, List<UnitData> allUnits, TeamRoster roster)
    {
        assistantIndex = index;
        gameObject.SetActive(true);

        foreach (var e in spawned)
            if (e != null) Destroy(e.gameObject);
        spawned.Clear();

        //exclude units already slotted
        var usedUnits = new HashSet<UnitData>();
        if (!roster.leaderSlot.IsEmpty)
            usedUnits.Add(roster.leaderSlot.unit);
        foreach (var s in roster.assistantSlots)
            if (s != null && !s.IsEmpty) usedUnits.Add(s.unit);

        foreach (var unit in allUnits)
        {
            if (unit.isLeader) continue;      //leaders can't be assistants
            if (usedUnits.Contains(unit)) continue;

            var entry = Instantiate(entryPrefab, container);
            entry.Setup(unit, index);
            spawned.Add(entry);
        }

        //add a "clear slot" entry at the top if player wants to do a challenge or smth
        var clearEntry = Instantiate(entryPrefab, container);
        clearEntry.SetupClear(index);
        clearEntry.transform.SetAsFirstSibling();
        spawned.Add(clearEntry);
    }

    public void Close() => gameObject.SetActive(false);
}
