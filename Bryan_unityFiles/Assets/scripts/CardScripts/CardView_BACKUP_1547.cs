using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;


public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;

    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Debug.Log("Setting up card: " + (card == null ? "null" : card.Title));
        Card = card;

        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();
<<<<<<< HEAD

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
=======
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
>>>>>>> 7ed08ae0571f5f8ed229d9b7d520408308b425ae
