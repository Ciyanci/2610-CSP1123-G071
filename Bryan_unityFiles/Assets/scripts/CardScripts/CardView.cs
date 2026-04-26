using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour
{
    //to make it show in unity inspector
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private Player player;


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
    }

    void OnMouseDown()
    {
        if (cardData != null)
        {
            player.PlayCard(cardData);
            testSystem.DiscardCard(card);
            Destroy(gameObject);
        }
    }
}
