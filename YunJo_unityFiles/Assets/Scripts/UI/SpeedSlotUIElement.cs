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
    public TextMeshProUGUI cardNameText;   // assigned card name, can be null
    public Image background;
    public Image clashIcon;                // ⚔ sprite, toggled by preview
    public Image cardThumbnail;            // optional artwork
    [Header("State Colors")]
    public Color emptyColor     = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color plannedColor   = new Color(0.2f, 0.5f, 0.8f, 0.9f);
    public Color committedColor = new Color(0.1f, 0.7f, 0.3f, 0.9f);
    public Color executedColor  = new Color(0.4f, 0.4f, 0.4f, 0.5f);
    public Color clashColor     = new Color(0.8f, 0.2f, 0.2f, 0.9f);
    [Header("Clash Sprites")]
    public Sprite clashSprite;
    public Sprite unopposedSprite;
    // Bound slot
    public SpeedSlot slot { get; private set; }
    public void Bind(SpeedSlot s)
    {
        slot      = s;
        slot.ui   = this;
        Show();
        Refresh();
    }
    // =========================
    // REFRESH — driven by slot state
    // =========================
    public void Refresh()
    {
        if (slot == null) return;
        // Value
        valueText.text = slot.value.ToString();
        // Card name
        if (cardNameText != null)
        {
            cardNameText.text = slot.assignedCard != null
                ? slot.assignedCard.Name
                : string.Empty;
        }
        // Artwork thumbnail
        if (cardThumbnail != null)
        {
            cardThumbnail.enabled = slot.assignedCard?.Artwork != null;
            if (slot.assignedCard?.Artwork != null)
                cardThumbnail.sprite = slot.assignedCard.Artwork;
        }
        // Background colour by state
        if (background != null)
        {
            background.color = slot.state switch
            {
                SlotState.Planned   => plannedColor,
                SlotState.Committed => committedColor,
                SlotState.Executed  => executedColor,
                _                   => emptyColor
            };
        }
        // Hide clash icon by default — preview system sets it
        if (clashIcon != null)
            clashIcon.enabled = false;
    }
    // =========================
    // PREVIEW — called by CombatFlowController
    // =========================
    public void ShowPreview(TargetPreviewState state)
    {
        if (clashIcon == null) return;
        switch (state)
        {
            case TargetPreviewState.WillClash:
                clashIcon.enabled = true;
                clashIcon.sprite  = clashSprite;
                if (background != null) background.color = clashColor;
                break;
            case TargetPreviewState.Unopposed:
                clashIcon.enabled = true;
                clashIcon.sprite  = unopposedSprite;
                if (background != null) background.color = plannedColor;
                break;
            default:
                clashIcon.enabled = false;
                break;
        }
    }
    // =========================
    // DROP — card dragged onto this slot
    // Slot owner is the player; target must come from the arrow/targeting system
    // =========================
    public void OnDrop(PointerEventData eventData)
    {
        if (slot == null || slot.owner == null) return;
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed) return;
        var cardView = eventData.pointerDrag?.GetComponent<CardView>();
        if (cardView == null) return;
        // Ask the flow controller to confirm with current target
        CombatFlowController.Instance.ConfirmTargetOnSlot(
            cardView.GetCard(),
            slot.owner,
            slot
        );
    }
    //assign and unassign
    public void OnPointerClick(PointerEventData eventData)
    {
        // Left click — select this slot for card assignment
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CombatFlowController.Instance.SelectSlot(slot);
            return;
        }

        // Right click — unassign
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slot == null || slot.state != SlotState.Planned) return;

            if (slot.assignedCard != null && slot.owner?.deck != null)
                slot.owner.deck.ReturnToHand(slot.assignedCard);

            slot.Clear();
            CombatFlowController.Instance.RefreshHandIfSelected(slot.owner);
        }
    }
    // =========================
    // VISIBILITY
    // =========================
    public void Show()
    {
        if (group != null)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    public void Hide()
    {
        if (group != null)
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Add to the existing IDropHandler, IPointerClickHandler list:
    // Also implement IPointerClickHandler for slot selection

    public Color selectedColor = new Color(1f, 0.85f, 0.1f, 1f); // gold highlight

    public void SetSelected(bool selected)
    {
        if (background == null) return;

        background.color = selected
            ? selectedColor
            : GetColorForState(slot?.state ?? SlotState.Empty);
    }

    Color GetColorForState(SlotState state)
    {
        return state switch
        {
            SlotState.Planned   => plannedColor,
            SlotState.Committed => committedColor,
            SlotState.Executed  => executedColor,
            _                   => emptyColor
        };
    }

}