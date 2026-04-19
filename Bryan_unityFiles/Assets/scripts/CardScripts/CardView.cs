using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;


public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        Debug.Log("Setting up card: " + (card == null ? "null" : card.Title));
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();
        imageSR.sprite = card.Image;
    } 

     public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();

        transform.localScale = Vector3.one * 1.1f;
        transform.position += Vector3.up * 1.5f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();

        transform.localScale = Vector3.one;
        transform.position -= Vector3.up * 1.5f;
    }
}
