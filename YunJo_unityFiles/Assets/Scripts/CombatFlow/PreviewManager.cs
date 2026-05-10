using UnityEngine;
using System.Collections.Generic;

public class PreviewManager : MonoBehaviour
{
    public static PreviewManager Instance;

    public ArrowController arrowPrefab;

    Dictionary<SpeedSlot, ArrowController> arrows = new();

    void Awake()
    {
        Instance = this;
    }

    public void Bind(CharacterUnit unit)
    {
        foreach (var slot in unit.speedSlots)
        {
            slot.onChanged += HandleSlotChanged;
        }
    }

    void HandleSlotChanged(SpeedSlot slot)
    {
        if (slot == null || slot.target == null || slot.assignedCard == null)
            return;

        if (!arrows.TryGetValue(slot, out ArrowController arrow))
        {
            arrow = Instantiate(arrowPrefab, transform);
            arrows[slot] = arrow;
        }

        arrow.Set(slot.target.headAnchor, slot.target.headAnchor);
        arrow.SetPriority(slot.value);
    }

    public void RemoveSlot(SpeedSlot slot)
    {
        if (arrows.TryGetValue(slot, out var arrow))
        {
            Destroy(arrow.gameObject);
            arrows.Remove(slot);
        }
    }

    public void Clear()
    {
        foreach (var a in arrows.Values)
            Destroy(a.gameObject);

        arrows.Clear();
    }
}