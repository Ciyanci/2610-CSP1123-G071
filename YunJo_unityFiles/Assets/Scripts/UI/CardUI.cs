using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CombatFlowController.Instance.StartTargeting(card, null);
    }

    // ✅ REQUIRED (even if unused)
    public void OnDrag(PointerEventData eventData)
    {
        // Intentionally empty
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CombatFlowController.Instance.EndTargeting();
    }
}