using UnityEngine;

[System.Serializable]
public class SpeedSlot
{
    public CharacterUnit owner;

    public int value;

    public Card assignedCard;
    public CharacterUnit target;

    public SlotState state = SlotState.Empty;

    public SpeedSlotUIElement ui;

    public void Roll()
    {
        value = Random.Range(1, 10);
        state = SlotState.Empty;
        assignedCard = null;
        target = null;

        ui?.Refresh();
    }

    public void Plan(Card card, CharacterUnit tgt)
    {
        assignedCard = card;
        target = tgt;
        state = SlotState.Planned;

        ui?.Refresh();
    }

    public void Commit()
    {
        if (state == SlotState.Planned)
            state = SlotState.Committed;

        ui?.Refresh();
    }

    public void Clear()
    {
        assignedCard = null;
        target = null;
        state = SlotState.Empty;

        ui?.Refresh();
    }

    public void Unassign()
    {
        Clear();
    }

    public void ResetTurn()
    {
        Clear();
    }
}