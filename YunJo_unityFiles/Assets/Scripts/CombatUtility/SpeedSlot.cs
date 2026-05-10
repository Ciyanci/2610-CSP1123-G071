using UnityEngine;

[System.Serializable]
public class SpeedSlot
{
    public int value;

    public SlotState state;
    public System.Action<SpeedSlot> onChanged;

    public Card assignedCard;
    public CharacterUnit target;
    public CharacterUnit owner;

    public SpeedDiceUI ui;

    public bool IsEmpty => assignedCard == null;

    public void Roll()
    {
        value = Random.Range(1, 10);

        if (state != SlotState.Committed)
            state = SlotState.Empty;

        ui?.SetValue(value);
        ui?.Show();

        onChanged?.Invoke(this);
    }

    public void Assign(Card card, CharacterUnit target, CharacterUnit owner)
    {
        if (state == SlotState.Executed)
            return;

        // prevent stealing unless explicitly overwritten later
        if (this.owner != null && this.owner != owner)
            return;

        this.owner = owner;

        assignedCard = card;
        this.target = target;
        state = SlotState.Planned;

        onChanged?.Invoke(this);
    }

    public void Commit()
    {
        if (state != SlotState.Planned)
            return;

        state = SlotState.Committed;

        onChanged?.Invoke(this);
    }

    public void Execute()
    {
        if (state != SlotState.Committed)
            return;

        state = SlotState.Executed;

        onChanged?.Invoke(this);
    }

    public void Unassign(CharacterUnit requester)
    {
        if (owner != null && owner != requester)
            return;

        assignedCard = null;
        target = null;
        owner = null;
        state = SlotState.Empty;

        onChanged?.Invoke(this);
    }

    public void ResetTurn()
    {
        assignedCard = null;
        target = null;
        state = SlotState.Empty;

        onChanged?.Invoke(this);
    }
}