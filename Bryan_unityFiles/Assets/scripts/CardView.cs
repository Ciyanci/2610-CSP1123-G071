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

    public Card Card {get; private set;}
    
    //method to display cards on the screen. It also updates the UI for the cards
    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();
        imageSR.sprite = card.Image;
    } 
}
