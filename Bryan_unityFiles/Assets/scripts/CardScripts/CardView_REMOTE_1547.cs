using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;

    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Card = card;

        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();

        if (card.Image != null)
            imageSR.sprite = card.Image;
    }

    void OnMouseDown()
    {
        var flow = FindFirstObjectByType<BattleFlowController>();

        if (flow != null)
        {
            flow.PlayCardFromUI(Card);
        }
    }
}