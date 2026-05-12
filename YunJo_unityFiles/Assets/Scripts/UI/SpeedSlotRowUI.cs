using UnityEngine;
using System.Collections.Generic;

public class SpeedSlotRowUI : MonoBehaviour
{
    public CharacterUnit owner;
    public List<SpeedSlotUIElement> slotUI = new();

    public Transform followTarget;
    public Vector3 offset = new Vector3(0, 1.2f, 0);

    public void AttachTo(CharacterUnit unit)
    {
        owner = unit;
        followTarget = unit.headAnchor;

        Bind(unit);
    }

    void LateUpdate()
    {
        if (followTarget == null) return;

        transform.position = followTarget.position + offset;
    }

    public void Bind(CharacterUnit unit)
    {
        owner = unit;

        followTarget = unit.headAnchor; // ADD THIS LINE

        slotUI.Clear();
        GetComponentsInChildren(slotUI);

        for (int i = 0; i < slotUI.Count; i++)
        {
            bool active = i < owner.speedSlots.Count;

            slotUI[i].gameObject.SetActive(active);

            if (active)
            {
                slotUI[i].Bind(owner.speedSlots[i]);
                slotUI[i].Show();
            }
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