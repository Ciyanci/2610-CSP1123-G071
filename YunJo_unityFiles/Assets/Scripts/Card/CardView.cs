using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text cost;
    public TMP_Text dice;

    Card card;
    CharacterUnit owner;

    Vector3 baseScale;
    Vector3 expandedScale = new Vector3(1.6f, 1f, 1f);

    public void Setup(Card c, CharacterUnit unit)
    {
        card = c;
        owner = unit;

        title.text = c.Data.Name;
        desc.text = c.Data.Description;
        cost.text = c.Cost.ToString();
        dice.text = $"{c.Min}-{c.Max}";

        baseScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        transform.DOScale(expandedScale, 0.15f);
    }

    void OnMouseExit()
    {
        transform.DOScale(baseScale, 0.15f);
    }

    void OnMouseDown()
    {
        CombatFlowController.Instance.SelectCard(card, owner);
        Debug.Log($"[CARD] Clicked: {card.Data.Name}");
    }
}