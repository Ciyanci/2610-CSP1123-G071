using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;

    [Header("References")]
    public Transform container;
    public CardView  prefab;

    [Header("Layout")]
    public float cardSpacing  = 160f; 
    public float hoverLiftY   = 60f;  
    public float neighbourPush = 40f;  

    List<CardView> views = new();
    CharacterDeck  currentDeck;
    CardView       currentHovered;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    //show/hide methods
    public void Show(CharacterDeck deck)
    {
        if (deck == null) return;
        gameObject.SetActive(true);
        Refresh(deck);
    }

    public void Hide()
    {
        Clear();
        gameObject.SetActive(false);
    }

    public RectTransform GetCardUI(CharacterUnit owner, Card card)
    {
        foreach (var v in views)
            if (v != null && v.GetCard() == card)
                return v.GetComponent<RectTransform>();
        return null;
    }

    //refresh deck method
    public void Refresh(CharacterDeck deck)
    {
        currentDeck = deck;
        Clear();

        var hand   = deck.GetHand();
        float startX = -(hand.Count - 1) * cardSpacing * 0.5f;

        for (int i = 0; i < hand.Count; i++)
        {
            CardView v = Instantiate(prefab, container);
            v.Setup(hand[i], deck.owner);
            v.SetBasePosition(new Vector2(startX + i * cardSpacing, 0));
            views.Add(v);
        }
    }

    //hover (only root card receives so it should fire cleanly) please i hope it does lowk i cant do this anymore
    public void OnCardHovered(CardView hovered)
    {
        if (currentHovered == hovered) return;

        // Reset previous
        if (currentHovered != null)
        {
            currentHovered.SetHover(false);
            currentHovered.ResetToBase();
        }

        currentHovered = hovered;
        currentHovered.BringToFront();
        int index = views.IndexOf(hovered);

        for (int i = 0; i < views.Count; i++)
        {
            var v = views[i];

            if (v == hovered)
            {
                //lifts hovered card upwards
                v.SetHover(true);
                v.LiftTo(hoverLiftY);
                continue;
            }

            //push neighbours (might wanna tweak it because expanded panel counts as the card and it looks weird)
            int   delta = i - index;
            float sign  = delta > 0 ? 1f : -1f;
            float dist  = Mathf.Abs(delta);
            float push  = sign * (neighbourPush + (dist - 1) * 15f);

            v.ApplyShift(push);
        }
    }

    public void ShowCards(List<Card> cards)
    {
        gameObject.SetActive(true);
        Clear();

        float startX = -(cards.Count - 1)*cardSpacing * 0.5f;
        for (int i = 0; i < cards.Count; i++)
        {
            CardView v = Instantiate(prefab, container);
            v.Setup(cards[i], null);
            v.SetBasePosition(new Vector2(startX+i*cardSpacing, 0));
            views.Add(v);
        }
    }

    public void ResetHover()
    {
        if (currentHovered == null) return;

        currentHovered.SetHover(false);
        currentHovered = null;

        foreach (var v in views)
            v.ResetToBase();

        currentHovered.RestoreSiblingOrder();
    }

    void Clear()
    {
        foreach (var v in views)
            if (v != null) Destroy(v.gameObject);
        views.Clear();
        currentHovered = null;
    }

    public void ShowLibraryCards(List<Card> cards)
    {
        gameObject.SetActive(true);
        Clear();

        int columns = 4;
        float xSpacing = cardSpacing;
        float ySpacing = 240f;

        int rows = Mathf.CeilToInt(cards.Count / (float)columns);

        for (int row = 0; row < rows; row++)
        {
            int cardsInRow = Mathf.Min(columns, cards.Count - row * columns);
            float startX = - (cardsInRow -1) * xSpacing * 0.5f;

            for (int col = 0; col < cardsInRow; col++)
            {
                int index = row * columns + col;

                CardView v = Instantiate(prefab, container);
                v.Setup(cards[index], null);

                float x = startX + col * xSpacing;
                float y = -row * ySpacing;

                v.SetBasePosition(new Vector2(x,y));

                views.Add(v);
            }
        }
    }
}
