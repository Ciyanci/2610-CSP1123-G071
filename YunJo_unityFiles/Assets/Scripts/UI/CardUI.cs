using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CombatFlowController.Instance.StartTargeting(card, null);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) { }
}