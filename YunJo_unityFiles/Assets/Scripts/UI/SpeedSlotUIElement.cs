using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

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

    //drop
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

    //click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //if card selected, choose this slot go brr
            if (CombatFlowController.Instance.IsTargeting)
            {
                CombatFlowController.Instance.SelectSlot(slot);
                return;
            }

            //otherwise if no card then show slot info
            CombatInfoBar.Instance?.ShowSlotInfo(slot);
            return;
        }

        //right click to unassign
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

    //visiblity
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

    //helpers
    public IEnumerator AnimateRoll(int finalValue, float duration = 0.8f)
    {
        float t = 0f;

        while (t < duration)
        {
            //show random numbers visually
            valueText.text = Random.Range(1, 10).ToString();
            t += Time.deltaTime;
            yield return null;
        }

        //locks in real value
        valueText.text = finalValue.ToString();
        Refresh();
    }
}
