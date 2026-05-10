using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedSlotDropZone : MonoBehaviour, IDropHandler
{
    public CharacterUnit owner;
    public SpeedSlot slot;

    // injected externally at click/hover time
    private CharacterUnit currentTarget;

    public void SetTarget(CharacterUnit target)
    {
        currentTarget = target;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var cardUI = eventData.pointerDrag?.GetComponent<CardUI>();
        if (cardUI == null) return;

        if (slot == null || owner == null || currentTarget == null)
            return;

        ActionPlanner.AssignToSlot(
            owner,
            slot,
            cardUI.card,
            currentTarget
        );
    }
}