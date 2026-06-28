using UnityEngine;
using System.Collections.Generic;

public class DebugInventoryFiller : MonoBehaviour
{
    [Header("All cards to add on start")]
    public List<CardData> allCards;

    void Awake()
    {
        foreach(var card in allCards)
            CardInventory.Add(card);
    }
}
