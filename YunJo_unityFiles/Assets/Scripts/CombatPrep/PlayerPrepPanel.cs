using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerPrepPanel : MonoBehaviour
{
    [Header("Slot Buttons")]
    public List<TeamSlotUI> slotUIs = new();

    [Header("Info Block")]
    public UnitInfoBlock infoBlock;

    [Header("Enter Battle")]
    public Button enterBattleButton;

    TeamRoster     roster;
    TeamRosterSlot selectedSlot;

    void Start()
    {
        enterBattleButton?.onClick.AddListener(
            () => CombatPrepManager.Instance?.EnterBattle());
    }

    public void Bind(TeamRoster r)
    {
        roster = r;

        //leader slot (index 0)
        if (slotUIs.Count > 0)
            slotUIs[0].BindPlayerSlot(
                r.leaderSlot,
                isLeader: true,
                assistantIndex: -1,
                onSelect: OnPlayerSlotSelected);

        //assistant slots
        for (int i = 1; i < slotUIs.Count; i++)
        {
            int ai = i - 1;
            if (ai < r.assistantSlots.Length)
                slotUIs[i].BindPlayerSlot(
                    r.assistantSlots[ai],
                    isLeader: false,
                    assistantIndex: ai,
                    onSelect: OnPlayerSlotSelected);
            else
                slotUIs[i].BindEmpty();
        }
    }

    void OnPlayerSlotSelected(TeamRosterSlot slot)
    {
        selectedSlot = slot;

        foreach (var s in slotUIs)
            s.SetSelected(s.BoundSlot == slot);

        infoBlock?.BindSlot(slot);
        CombatPrepManager.Instance?.SelectPlayerSlot(slot);
    }

    public void SetSelected(TeamRosterSlot slot)
    {
        OnPlayerSlotSelected(slot);
    }

    public void RefreshSelected()
    {
        infoBlock?.RefreshDeck();
    }
}
