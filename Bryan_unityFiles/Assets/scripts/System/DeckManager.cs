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
        Debug.Log("deck has been created, cards in draw pile:"+drawPile.Count);
        DrawHand();
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
            Debug.Log("no cards after reshuffle still");
             return;
        }
        //removes card from drawpile and puts it on your hand
        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        onHand.Add(card);
        Debug.Log("drew card: "+ card.Title + card.Description + card.Image);
        PrintDeckState();
    }

    public void DrawHand()
    {
        Debug.Log("drawing hand");

        for (int i =0; i < handSize; i++)
        {
            DrawCard();
        }
        Debug.Log("hand size after drawing" + onHand.Count);
        PrintDeckState();
    }

    public void Reshuffle()
    {
        //adds discardpile to drawpile, clears discardpile and shuffles drawpile
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }
    public void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            // randomises the list from i to list.count-1 (does not count numbers before i)
            int rand = UnityEngine.Random.Range(i, list.Count);
            // swaps i with the random number given
            (list [i], list[rand]) = (list[rand], list[i]);
        }
    }
    public void EndTurn()
    {
        discardPile.AddRange(onHand);
        onHand.Clear();
        Shuffle(drawPile);
        PrintDeckState();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            DrawCard();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            DrawHand();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            EndTurn();
        }
    }
    void PrintDeckState()
    {
        Debug.Log("draw: " + drawPile.Count);
        Debug.Log("hand: " + onHand.Count);
        Debug.Log("discard "+ discardPile.Count);
    }

    

}
