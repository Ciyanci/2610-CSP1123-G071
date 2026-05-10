using UnityEngine;
using System.Collections.Generic;

public class CharacterDeck : MonoBehaviour
{
    public CharacterUnit owner;

    public List<CardData> startingDeck;

    List<Card> drawPile = new();
    List<Card> discardPile = new();
    List<Card> hand = new();

    public int handSize = 9;

    public void Init()
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        foreach (var data in startingDeck)
        {
            drawPile.Add(new Card(data));
        }

        Shuffle(drawPile);
    }

    public void FillHandToLimit()
    {
        while (hand.Count < handSize)
        {
            Card c = Draw();

            if (c == null)
                break;

            hand.Add(c);
        }
    }

    public Card Draw()
    {
        if (drawPile.Count == 0)
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();

            Shuffle(drawPile);
        }

        if (drawPile.Count == 0)
            return null;

        Card c = drawPile[0];

        drawPile.RemoveAt(0);

        return c;
    }

    public void UseCard(Card card)
    {
        if (!hand.Contains(card))
            return;

        hand.Remove(card);
        discardPile.Add(card);
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);

            (list[i], list[r]) =
                (list[r], list[i]);
        }
    }
}