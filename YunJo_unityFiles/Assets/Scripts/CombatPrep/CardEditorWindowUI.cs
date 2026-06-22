using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardEditorWindowUI : MonoBehaviour
{
    [Header("Current Deck")]
    public Transform       currentDeckContainer;
    public DeckCardEntryUI currentDeckEntryPrefab;

    [Header("Available Pool")]
    public Transform       poolContainer;
    public CardListEntryUI poolEntryPrefab;

    [Header("Close")]
    public Button closeButton;

    List<DeckCardEntryUI> spawnedCurrent = new();
    List<CardListEntryUI> spawnedPool    = new();

    CharacterUnit  boundUnit;
    List<CardData> allCards;

    void Awake() => closeButton?.onClick.AddListener(Close);

    public void Open(CharacterUnit unit, List<CardData> cards)
    {
        boundUnit = unit;
        allCards  = cards;
        gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        RefreshCurrentDeck();
        RefreshPool();
    }

    void RefreshCurrentDeck()
    {
        foreach (var e in spawnedCurrent)
            if (e != null) Destroy(e.gameObject);
        spawnedCurrent.Clear();

        if (boundUnit?.deck == null || currentDeckEntryPrefab == null) return;

        foreach (var card in boundUnit.deck.startingDeck)
        {
            var entry = Instantiate(currentDeckEntryPrefab, currentDeckContainer);
            entry.Setup(card, boundUnit, isInteractable: true);
            var c = card; // capture for lambda
            entry.button?.onClick.RemoveAllListeners();
            entry.button?.onClick.AddListener(() =>
                CombatPrepManager.Instance?.RemoveCard(boundUnit, c));
            spawnedCurrent.Add(entry);
        }
    }

    void RefreshPool()
    {
        foreach (var e in spawnedPool)
            if (e != null) Destroy(e.gameObject);
        spawnedPool.Clear();

        if (boundUnit?.unitData == null || poolEntryPrefab == null) return;

        var pool = boundUnit.unitData.GetFullCardPool(boundUnit.equippedKeypage);

        foreach (var card in pool)
        {
            var entry   = Instantiate(poolEntryPrefab, poolContainer);
            bool inDeck = boundUnit.deck.startingDeck.Contains(card);
            entry.Setup(card, inDeck, boundUnit);
            spawnedPool.Add(entry);
        }
    }

    public void Close() => gameObject.SetActive(false);
}
