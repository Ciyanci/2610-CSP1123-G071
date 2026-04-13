using UnityEngine;
using System;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{

    public List<CardData> startingDeck;

    private List<Card> drawPile = new();
    private List<Card> onHand = new();
    private List<Card> discardPile = new();
    private int handSize = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeDeck();
        Shuffle(drawPile);
    }

    void InitializeDeck()
    {
        
        foreach (var data in startingDeck)
        {
            drawPile.Add(new Card(data));
        }
    }
    void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Reshuffle();
        }
        if (drawPile.Count == 0)
        {
             return;
        }
        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        onHand.Add(card);
    }

    public void DrawHand()
    {
        for (int i =0; i < handSize; i++)
        {
            DrawCard();
        }
    }

    public void Reshuffle()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }
    public void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, list.Count);
            (list [i], list[rand]) = (list[rand], list[i]);
        }
    }
    public void EndTurn()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }

}
