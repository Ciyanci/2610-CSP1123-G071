using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text cost;
    public TMP_Text dice;
    public UnityEngine.UI.Image artwork;

    Card card;
    CharacterUnit owner;

    Vector3 baseScale;
    Vector3 expandedScale = new(1.6f, 1f, 1f);

    bool dragging = false;

    public void Setup(Card c, CharacterUnit unit)
    {
        card = c;
        owner = unit;

        title.text = c.Data.Name;
        desc.text = c.Data.Description;
        cost.text = c.Cost.ToString();
        dice.text = $"{c.Min}-{c.Max}";
        artwork.sprite = c.Artwork;
        artwork.preserveAspect = true;

        baseScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        transform.DOScale(expandedScale, 0.15f);
    }

    void OnMouseExit()
    {
        if (!dragging)
            transform.DOScale(baseScale, 0.15f);
    }

    void OnMouseDown()
    {
        CombatFlowController.Instance.StartTargeting(card, owner);

        Debug.Log($"[CARD] Drag start: {card.Data.Name}");
    }

    void OnMouseUp()
    {
        dragging = false;
        transform.DOScale(baseScale, 0.1f);
    }
}