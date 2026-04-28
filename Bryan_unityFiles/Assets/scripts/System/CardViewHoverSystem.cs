using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    [SerializeField] private CardView cardViewHover;
    public void Show(CardData cardData, Vector3 position)
    {
        Debug.Log(cardData == null ? "card null" : "card working");

        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(cardData);
        cardViewHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
    }
}
