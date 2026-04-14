using UnityEngine;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public HandView handView;

    public Card Draw()
    {
        Card c = cards[Random.Range(0, cards.Count)];

        // spawn visual card
        if (handView != null && CardViewCreator.Instance != null)
        {
            CardView view = CardViewCreator.Instance.CreateCardView(Vector3.zero, Quaternion.identity);
            view.Setup(c);

            StartCoroutine(handView.AddCard(view));
        }

        return c;
    }
}