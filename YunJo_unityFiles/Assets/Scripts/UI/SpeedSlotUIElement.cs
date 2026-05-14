using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpeedSlotUIElement : MonoBehaviour,
    IDropHandler,
    IPointerClickHandler
{
    [Header("Refs")]
    public CanvasGroup group;
    public TextMeshProUGUI valueText;
    public Image background;

    public Image outline;

    [Header("State Colors")]
    public Color emptyColor     = new Color(0.15f, 0.15f, 0.15f, 0.85f);
    public Color plannedColor   = new Color(0.2f,  0.5f,  0.8f,  0.9f);
    public Color committedColor = new Color(0.1f,  0.7f,  0.3f,  0.9f);
    public Color executedColor  = new Color(0.35f, 0.35f, 0.35f, 0.5f);
    public Color selectedColor  = new Color(1f,    0.85f, 0.1f,  1f);

    public SpeedSlot slot { get; private set; }

    public void Bind(SpeedSlot s)
    {
        slot    = s;
        slot.ui = this;
        Show();
        Refresh();
    }

    public void Refresh()
    {
        if (slot == null) return;

        valueText.text = slot.value.ToString();

        if (background != null)
            background.color = GetColorForState(slot.state);

        if (outline != null)
            outline.color = GetColorForState(slot.state);
    }

    public void SetSelected(bool selected)
    {
        if (background == null) return;
        background.color = selected
            ? selectedColor
            : GetColorForState(slot?.state ?? SlotState.Empty);
        if (outline == null) return;
        outline.color = selected
            ? selectedColor
            : GetColorForState(slot?.state ?? SlotState.Empty);
    }

    Color GetColorForState(SlotState state) => state switch
    {
        SlotState.Planned   => plannedColor,
        SlotState.Committed => committedColor,
        SlotState.Executed  => executedColor,
        _                   => emptyColor
    };

    // =========================
    // DROP
    // =========================
    public void OnDrop(PointerEventData eventData)
    {
        if (slot == null || slot.owner == null) return;
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed) return;

        var cardView = eventData.pointerDrag?.GetComponent<CardView>();
        if (cardView == null) return;

        CombatFlowController.Instance.ConfirmTargetOnSlot(
            cardView.GetCard(),
            slot.owner,
            slot
        );
    }

    // =========================
    // CLICK
    // =========================
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // If a card is being targeted, select this slot
            if (CombatFlowController.Instance.IsTargeting)
            {
                CombatFlowController.Instance.SelectSlot(slot);
                return;
            }

            // Otherwise open the info bar for this slot's interaction
            CombatInfoBar.Instance?.ShowSlotInfo(slot);
            return;
        }

        // Right click — unassign
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slot.state != SlotState.Planned) return;

            if (slot.assignedCard != null && slot.owner?.deck != null)
                slot.owner.deck.ReturnToHand(slot.assignedCard);

            slot.Clear();
            CombatFlowController.Instance.RefreshHandIfSelected(slot.owner);
            CombatInfoBar.Instance?.Clear();
            ArrowManager.Instance?.RemovePlannedArrow(slot);
        }
    }

    // =========================
    // VISIBILITY
    // =========================
    public void Show()
    {
        if (group != null)
        {
            group.alpha          = 1;
            group.interactable   = true;
            group.blocksRaycasts = true;
        }
        else gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (group != null)
        {
            group.alpha          = 0;
            group.interactable   = false;
            group.blocksRaycasts = false;
        }
        else gameObject.SetActive(false);
    }
}
