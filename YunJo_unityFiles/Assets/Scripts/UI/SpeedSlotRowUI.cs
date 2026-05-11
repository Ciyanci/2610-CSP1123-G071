using UnityEngine;
using System.Collections.Generic;

public class SpeedSlotRowUI : MonoBehaviour
{
    public CharacterUnit owner;
    public List<SpeedSlotUIElement> slotUI = new();

    public void Bind(CharacterUnit unit)
    {
        owner = unit;

        // IMPORTANT: ensure same size
        if (owner.speedSlots.Count != slotUI.Count)
        {
            Debug.LogWarning("Mismatch: speed slots vs UI slots");
        }

        for (int i = 0; i < Mathf.Min(slotUI.Count, owner.speedSlots.Count); i++)
        {
            slotUI[i].Bind(owner.speedSlots[i]);
        }
    }

    public void Refresh()
    {
        if (owner == null) return;

        for (int i = 0; i < slotUI.Count && i < owner.speedSlots.Count; i++)
        {
            slotUI[i].Refresh();
        }
    }
}