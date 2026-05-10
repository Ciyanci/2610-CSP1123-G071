using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;

    [Header("References")]
    public Transform container;
    public CardView prefab;

    [Header("Layout")]
    public float spacing = 340f;
    public float pushAmount = 260f;

    List<CardView> views = new();

    CharacterDeck currentDeck;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(CharacterDeck deck)
    {
        currentDeck = deck;

        gameObject.SetActive(true);

        Refresh(deck);
    }

    public void Refresh(CharacterDeck deck)
    {
        currentDeck = deck;

        Clear();

        List<Card> hand = deck.GetHand();

        float startX = -(hand.Count - 1) * spacing * 0.5f;

        for (int i = 0; i < hand.Count; i++)
        {
            CardView v = Instantiate(prefab, container);

            RectTransform rect = v.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(
                startX + i * spacing,
                0
            );

            v.Setup(hand[i], deck.owner);

            views.Add(v);
        }
    }

    // =========================
    // CARD HOVER SPACING
    // =========================

    public void OnCardHovered(CardView hovered)
    {
        int hoveredIndex = views.IndexOf(hovered);

        float shift = 260f;

        for (int i = 0; i < views.Count; i++)
        {
            if (views[i] == hovered)
                continue;

            if (i > hoveredIndex)
                views[i].Shift(shift);
            else
                views[i].ResetShift();
        }
    }

    public void ResetHover()
    {
        foreach (var card in views)
        {
            card.ResetShift();
            card.ResetCard();
        }
    }

    public void FocusCard(CardView focused)
    {
        int index = views.IndexOf(focused);

        for (int i = 0; i < views.Count; i++)
        {
            if (views[i] == focused)
                continue;

            if (i < index)
                views[i].Shift(-pushAmount);
            else
                views[i].Shift(pushAmount);
        }
    }

    public void ResetFocus()
    {
        foreach (var v in views)
        {
            v.ResetShift();
                        v.ResetCard();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Clear();
    }

    public void Clear()
    {
        foreach (var v in views)
        {
            if (v != null)
                Destroy(v.gameObject);
        }

        views.Clear();
    }
}