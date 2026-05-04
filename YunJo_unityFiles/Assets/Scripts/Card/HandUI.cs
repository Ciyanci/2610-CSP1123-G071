using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    public Transform container;
    public CardView prefab;

    List<CardView> views = new();

    public void Show(CharacterDeck deck)
    {
        Clear();

        foreach (var c in deck.GetHand())
        {
            var v = Instantiate(prefab, container);
            v.Setup(c, deck.owner);
            views.Add(v);
        }
    }

    public void Clear()
    {
        foreach (var v in views)
            Destroy(v.gameObject);

        views.Clear();
    }
}