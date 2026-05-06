using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;

    public Transform container;
    public CardView prefab;

    List<CardView> views = new();

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(CharacterDeck deck)
    {
        gameObject.SetActive(true);
        Clear();

        var hand = deck.GetHand(); // already capped to 4

        foreach (var c in hand)
        {
            var v = Instantiate(prefab, container);
            v.Setup(c, deck.owner);
            views.Add(v);
        }
    }

    public void Refresh(CharacterDeck deck)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Clear();

        foreach (var c in deck.GetHand())
        {
            var v = Instantiate(prefab, container);
            v.Setup(c, deck.owner);
            views.Add(v);
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
            Destroy(v.gameObject);

        views.Clear();
    }
}