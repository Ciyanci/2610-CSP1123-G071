using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterDeck : MonoBehaviour
{
    public CharacterUnit owner;

    [Header("Deck Setup")]
    public List<CardData> startingDeck;

    [Header("Runtime")]
    public List<Card> drawPile = new();
    public List<Card> discardPile = new();
    public List<Card> hand = new();

    [Header("Rules")]
    public int maxSelectableCards = 9;
    public int maxUsableCards = 4;

    void Start()
    {
        Init();
    }

    // =========================
    // INIT
    // =========================
    public void Init()
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        foreach (var data in startingDeck)
            drawPile.Add(new Card(data));

        Shuffle(drawPile);

        FillHandToLimit();
    }

    // =========================
    // CORE HAND SYSTEM
    // =========================
    public void FillHandToLimit()
    {
        while (hand.Count < maxSelectableCards)
        {
            Card next = DrawFromPile();

            if (next == null)
                break;

            hand.Add(next);
        }
    }

    Card DrawFromPile()
    {
        if (drawPile.Count == 0)
            ReshuffleDiscardIntoDeck();

        if (drawPile.Count == 0)
            return null;

        var card = drawPile[0];
        drawPile.RemoveAt(0);

        return card;
    }

    // =========================
    // CARD USAGE (NO DISCARD FORCE)
    // =========================
    public void UseCard(Card c)
    {
        if (!hand.Contains(c)) return;

        hand.Remove(c);
        discardPile.Add(c);

        Debug.Log($"[DECK] Used {c.Data.Name}");

        // auto refill slot
        FillHandToLimit();
    }

    // =========================
    // RESHUFFLE ONLY WHEN EMPTY
    // =========================
    void ReshuffleDiscardIntoDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();

        Shuffle(drawPile);
    }

    // =========================
    // SHUFFLE
    // =========================
    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    // =========================
    // UI ACCESS
    // =========================
    public List<Card> GetHand()
    {
        return hand.Take(maxUsableCards).ToList();
    }

    public void OpenDeck()
    {
        HandUI.Instance.Show(this);
    }
}