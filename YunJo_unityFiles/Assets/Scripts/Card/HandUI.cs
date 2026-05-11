using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;

    [Header("References")]
    public Transform container;
    public CardView prefab;

    [Header("Layout")]
    public float spacing = 500f;

    List<CardView> views = new();

    CharacterDeck currentDeck;

    CardView currentHovered;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    // =========================
    // REQUIRED FIX (for CombatFlowController)
    // =========================
    public void Show(CharacterDeck deck)
    {
        if (deck == null)
            return;

        gameObject.SetActive(true);
        Refresh(deck);
    }

    public RectTransform GetCardUI(CharacterUnit owner, Card card)
    {
        foreach (var v in views)
        {
            if (v != null && v.GetCard() == card)
                return v.GetComponent<RectTransform>();
        }
        return null;
    }

    public void Hide()
    {
        Clear();
        gameObject.SetActive(false);
    }

    // =========================
    // CORE
    // =========================
    public void Refresh(CharacterDeck deck)
    {
        currentDeck = deck;

        Clear();

        var hand = deck.GetHand();

        float startX = -(hand.Count - 1) * spacing * 0.5f;

        for (int i = 0; i < hand.Count; i++)
        {
            CardView v = Instantiate(prefab, container);
            v.Setup(hand[i], deck.owner);

            v.SetBasePosition(new Vector2(startX + i * spacing, 0));

            views.Add(v);
        }
    }

    public void OnCardHovered(CardView hovered)
    {
        if (currentHovered == hovered)
            return;

        currentHovered = hovered;

        int index = views.IndexOf(hovered);

        for (int i = 0; i < views.Count; i++)
        {
            var v = views[i];

            v.ResetToBase();

            if (v == hovered)
            {
                v.SetHover(true);
                continue;
            }

            int offset = i - index;

            float shift = offset > 0 ? 260f + offset * 35f : offset * 25f;

            v.ApplyShift(shift);
        }
    }

    public void ResetHover()
    {
        currentHovered = null;

        foreach (var v in views)
            v.ResetToBase();
    }
    void Clear()
    {
        foreach (var v in views)
            if (v != null)
                Destroy(v.gameObject);

        views.Clear();
    }
}