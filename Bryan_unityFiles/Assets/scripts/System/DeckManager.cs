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
        Debug.Log("Deck has been created, Cards in draw pile:"+drawPile.Count);
    }

    void InitializeDeck()
    {
        Debug.Log("starting deck size:" + startingDeck.Count);
        foreach (var data in startingDeck)
        {
            drawPile.Add(new Card(data));
        }
        Debug.Log("draw pile after init:" + drawPile.Count);
    }
    void DrawCard()
    {
        Debug.Log("attempting to draw card");

        if (drawPile.Count == 0)
        {
            Debug.Log("draw pile empty, reshuffling");
            Reshuffle();
        }
        if (drawPile.Count == 0)
        {
            Debug.Log("no cards after reshuffle stil");
             return;
        }
        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        onHand.Add(card);
        Debug.Log("drew card: "+ card.Title + card.Description + card.Image);
    }

    public void DrawHand()
    {
        Debug.Log("drawing hand");

        for (int i =0; i < handSize; i++)
        {
            DrawCard();
        }
        Debug.Log("hand size after drawing" + onHand.Count);
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            DrawHand();
        }
    }

    

}
