using System.Collections.Generic;
using UnityEngine;

//static session inventory — extend with file save if have enough time
public static class CardInventory
{
    static List<CardData> earned = new();

    public static void Add(CardData card)
    {
        if (!earned.Contains(card))
        {
            earned.Add(card);
            Debug.Log($"[INVENTORY] Added: {card.Name}");
        }
    }

    public static bool Has(CardData card) => earned.Contains(card);

    public static List<CardData> GetAll() => new List<CardData>(earned);

    public static void Clear() => earned.Clear();
}
