using UnityEngine;
using System.Collections.Generic;
public class CharacterDeck : MonoBehaviour
{
    public CharacterUnit owner;
    public List<CardData> startingDeck;
    List<Card> drawPile  = new();
    List<Card> discardPile = new();
    List<Card> hand      = new();
    [Header("Hand")]
    public int deckSize  = 9;
    public int handSize  = 4;
    //init() is called once at battle start btw
    public void Init()
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();
        foreach (var data in startingDeck)
            drawPile.Add(new Card(data));
        // Trim or pad to deckSize if needed
        while (drawPile.Count > deckSize)
            drawPile.RemoveAt(drawPile.Count - 1);
        Shuffle(drawPile);
    }
    //refresh hand (discard current hand, reshuffles, draws handSize fresh cards)
    public void RefreshHand()
    {
        // Return hand to discard
        discardPile.AddRange(hand);
        hand.Clear();
        FillHandToLimit();
    }
    //fill (tops up to handSize — used by RefreshHand and BattleStarter)
    public void FillHandToLimit()
    {
        while (hand.Count < handSize)
        {
            Card c = Draw();
            if (c == null) break;
            hand.Add(c);
        }
    }
    //draw (recycles discard into draw when empty)
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

    //load card list
    public void LoadFromCardList(List<CardData> cards)
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();
        foreach (var data in cards)
            drawPile.Add(new Card(data));
        Shuffle(drawPile);
        Debug.Log($"[DECK] Loaded {drawPile.Count} cards for {owner?.unitName}");
    }
    //useCard (moves a played card to discard)
    public void UseCard(Card card)
    {
        if (!hand.Contains(card))
            return;
        hand.Remove(card);
        discardPile.Add(card);
    }

    public void ReturnToHand(Card card)
    {
        if (card == null) return;
        if (hand.Contains(card)) return;
        hand.Add(card);
    }
    public List<Card> GetHand() => hand;

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}