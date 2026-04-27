using UnityEngine;
using System.Collections.Generic;


public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private List<CardData> startingDeck;
    [SerializeField] private PlayerCost energy;

    private List<Card> drawPile = new();
    private List<Card> hand = new();
    private List<Card> discardPile = new();

    private int handSize = 5;

    void Start()
    {
        foreach(var data in startingDeck)
        {
            drawPile.Add(new Card(data));
        }
        Shuffle(drawPile);
        StartTurn();
    }




    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            DrawCard();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            TryPlayFirstCard();
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity);
            Card card = new Card(cardData);
            cardView.Setup(cardData);
            StartCoroutine(handView.AddCard(cardView));
        }*/
    }

    void StartTurn()
    {
        energy.StartTurn();
        DrawHand();
    }

    void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Reshuffle();
        }

        if (drawPile.Count == 0)
        {
            Debug.Log("no cards left!");
            return;
        }

        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        hand.Add(card);
        SpawnCard(card);
        Debug.Log("Drew: " + card.Title);
    }

    void DrawHand()
    {
        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
    }

    void SpawnCard(Card card)
    {
        CardView view = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity);
        view.Setup(card.Data);
        view.Init(this, card);
        StartCoroutine(handView.AddCard(view));
    }

    void TryPlayFirstCard()
    {
        if (hand.Count == 0) return;
        Card card = hand[0];
        if (!energy.Spend(card.Cost))
        {
            Debug.Log("not enough energy");
            return;
        }

        hand.RemoveAt(0);
        discardPile.Add(card);
        handView.RemoveFirstCard();
    }

    void EndTurn()
    {
        discardPile.AddRange(hand);
        hand.Clear();
        handView.ClearHand();
        Debug.Log("end turn");
        StartTurn();
    }

   /* public void DiscardCard(Card card)
    {
        hand.Remove(card);
        discardPile.Add(card);
        Debug.Log("Discarded: " + card.Title);
    }*/

    /*public void EndTurn()
    {
        discardPile.AddRange(hand);
        hand.Clear();
        Debug.Log("End turn => discard all");
        DrawHand();
    }*/

    void Reshuffle()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
        Debug.Log("Reshufled deck");
    }

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void TryPlayCard(CardView view, Card card)
    {
        if (!energy.Spend(card.Cost))
        {
            Debug.Log("Not enough energy!");
            return;
        }

        Debug.Log("Played: " + card.Title);

        hand.Remove(card);
        discardPile.Add(card);

        handView.RemoveCard(view);

        Destroy(view.gameObject);
    }
}
