using UnityEngine;
using System.Collections.Generic;

public class LibrarySceneBootstrap : MonoBehaviour
{
    void Start()
    {
        List<Card> cards = new();

        foreach (CardData data in CardInventory.GetAll())
        {
            cards.Add(new Card(data));
        }
        HandUI.Instance.ShowLibraryCards(cards);
    }
}
