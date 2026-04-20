using UnityEngine;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    public List<Card> cards = new();
    public HandView handView;
    public CharacterUnit owner;

    List<Card> hand = new();

    void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<CharacterUnit>();
    }

    void OnMouseDown()
    {
        CombatInputController.Instance.SelectUnit(owner);
    }

    public Card Draw()
    {
        if (cards == null || cards.Count == 0)
            return null;

        return cards[Random.Range(0, cards.Count)];
    }

    public void OpenDeck()
    {
        ClearHand();
        DrawHand(5);
    }

    public void ClearHand()
    {
        hand.Clear();
        handView?.Clear();
    }

    public void DrawHand(int amount)
    {
        hand.Clear();

        bool hasZero = false;

        for (int i = 0; i < amount; i++)
        {
            Card c = cards[Random.Range(0, cards.Count)];
            if (c.Cost == 0) hasZero = true;
            hand.Add(c);
        }

        if (!hasZero)
        {
            Card zero = cards.Find(x => x.Cost == 0);
            if (zero != null && hand.Count > 0)
                hand[0] = zero;
        }

        foreach (var c in hand)
            SpawnCard(c);
    }

    void SpawnCard(Card c)
    {
        CardView view = CardViewCreator.Instance.CreateCardView();

        view.transform.SetParent(handView.transform, false);
        view.Setup(c, owner);

        view.onCardClicked = (_) =>
        {
            CombatInputController.Instance.SelectCard(view.Card, owner);
        };

        handView.Register(view);
    }
}