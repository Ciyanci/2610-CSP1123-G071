using UnityEngine;
using System.Collections.Generic;

public class SpeedSlotRowUI : MonoBehaviour
{
    public CharacterUnit owner;

    public List<SpeedSlotUIElement> slotUI = new();

    public void Bind(CharacterUnit unit)
    {
        owner = unit;

        Refresh();
    }

    public void Refresh()
    {
        if (owner == null) return;

        for (int i = 0; i < owner.speedSlots.Count; i++)
        {
            if (i >= slotUI.Count)
                continue;

            slotUI[i].Bind(owner.speedSlots[i]);
        }
    }

    void Update()
    {
        // lightweight auto-refresh (we can optimize later)
        if (owner != null)
            Refresh();
    }
}