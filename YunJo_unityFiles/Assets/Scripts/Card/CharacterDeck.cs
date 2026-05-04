using UnityEngine;
using System.Collections.Generic;

public class CharacterDeck : MonoBehaviour
{
    public CharacterUnit owner;

    [Header("Deck Setup")]
    public List<CardData> startingDeck;

    // 🔥 KEEP PUBLIC FOR OLD SCRIPTS (EnemyAI etc.)
    public List<Card> cards = new();

    List<Card> drawPile = new();
    List<Card> discard = new();

    public int handSize = 3;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        drawPile.Clear();
        discard.Clear();
        cards.Clear();

        foreach (var data in startingDeck)
        {
            Card c = new Card(data);
            drawPile.Add(c);
        }

        Shuffle(drawPile);
        DrawHand();
    }

    // 🔥 FIX: Draw must be PUBLIC (your error)
    public Card Draw()
    {
        if (drawPile.Count == 0)
            Reshuffle();

        if (drawPile.Count == 0)
            return null;

        Card c = drawPile[0];
        drawPile.RemoveAt(0);
        cards.Add(c);
        Debug.Log($"[DECK] {owner.name} drew {c.Data.Name}");

        return c;
    }

    public void DrawHand()
    {
        for (int i = 0; i < handSize; i++)
            Draw();
    }

    public List<Card> GetHand() => cards;

    public void DiscardOne()
    {
        if (cards.Count == 0) return;

        Card c = cards[0];
        cards.RemoveAt(0);
        discard.Add(c);
        Debug.Log($"[DECK] {owner.name} discarded a card");
    }

    void Reshuffle()
    {
        drawPile.AddRange(discard);
        discard.Clear();
        Shuffle(drawPile);
    }

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    // 🔥 COMPATIBILITY FIX
    public void OpenDeck()
    {
        var handUI = FindFirstObjectByType<HandUI>();
        if (handUI != null)
            handUI.Show(this);
    }
}