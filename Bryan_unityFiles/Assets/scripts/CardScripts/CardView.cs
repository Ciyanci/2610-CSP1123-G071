using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private Player player;

<<<<<<< HEAD
    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Card = card;

        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();

        if (card.Image != null)
            imageSR.sprite = card.Image;
=======

    public CardData cardData {get; private set;}
    private TestSystem testSystem;
    private Card card;

    public void Init(TestSystem system, Card c)
    {
        testSystem = system;
        card = c;
    }
    
    //method to display cards on the screen. It also updates the UI for the cards
    public void Setup(CardData data)
    {
        cardData = data;
        title.text = data.Name;
        description.text = data.Description;
        cost.text = data.Cost.ToString();
        imageSR.sprite = data.Image;
    } 

    void OnMouseEnter()
    {
        wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x -2, 0);
        CardViewHoverSystem.Instance.Show(cardData, pos);
    }
    
    void OnMouseExit()
    {
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
>>>>>>> 2f72c6e5ee10f44ca8e6760df476034df3dc4dc5
    }

    void OnMouseDown()
    {
<<<<<<< HEAD
        var flow = FindFirstObjectByType<BattleFlowController>();

        if (flow != null)
        {
            flow.PlayCardFromUI(Card);
=======
        if (cardData != null)
        {
            testSystem.TryPlayCard(this, card);
>>>>>>> 2f72c6e5ee10f44ca8e6760df476034df3dc4dc5
        }
    }
}