using UnityEngine;
using System.Collections.Generic;

public class LibrarySceneBootstrap : MonoBehaviour
{
    [Header ("Drag all your CardData ScriptableObjects here")]
    public List<CardData> allCards;

    void Start()
    {
        var cards = new List<Card>();
        foreach (var data in allCards)
            cards.Add(new Card(data));

        HandUI.Instance.ShowCards(cards);
    }
}
