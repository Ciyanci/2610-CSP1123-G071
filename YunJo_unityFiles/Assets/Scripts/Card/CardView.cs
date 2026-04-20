using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private SpriteRenderer frameSR;
    [SerializeField] private SpriteRenderer artworkSR;

    public Card Card { get; private set; }

    public System.Action<CardView> onHoverEnter;
    public System.Action<CardView> onHoverExit;
    public System.Action<CardView> onCardClicked;

    public CharacterUnit owner;

    Vector3 basePos;

    public void Setup(Card card, CharacterUnit unit)
    {
        Card = card;
        owner = unit;

        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();

        if (card.Image != null)
            artworkSR.sprite = card.Image;

        if (card.data != null && card.data.Frame != null)
            frameSR.sprite = card.data.Frame;
    }

    void Start()
    {
        basePos = transform.localPosition;
    }

    void OnMouseDown()
    {
        CombatFlowController.Instance.SelectCard(Card, owner);
    }

    void OnMouseEnter()
    {
        onHoverEnter?.Invoke(this);

        transform.DOKill();
        transform.DOLocalMove(basePos + Vector3.up * 0.5f, 0.15f);
        transform.DOScale(1.2f, 0.15f);
    }

    void OnMouseExit()
    {
        onHoverExit?.Invoke(this);

        transform.DOKill();
        transform.DOLocalMove(basePos, 0.15f);
        transform.DOScale(1f, 0.15f);
    }
}