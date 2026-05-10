using UnityEngine;
using TMPro;

public class SpeedSlotUIElement : MonoBehaviour
{
    public TextMeshProUGUI valueText;

    SpeedSlot slot;

    public void Bind(SpeedSlot slot)
    {
        this.slot = slot;
        Refresh();
    }

    public void Refresh()
    {
        if (slot == null) return;

        valueText.text = slot.value.ToString();

        valueText.color = GetColor(slot.state);
    }

    Color GetColor(SlotState state)
    {
        switch (state)
        {
            case SlotState.Empty:
                return Color.white;

            case SlotState.Planned:
                return new Color(0.6f, 0.8f, 1f); // light blue (preview)

            case SlotState.Committed:
                return Color.yellow;

            case SlotState.Executed:
                return Color.gray;

            default:
                return Color.white;
        }
    }
}