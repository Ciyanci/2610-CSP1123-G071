using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardEditorWindowUI : MonoBehaviour
{
    [Header("Current Deck")]
    public Transform       currentDeckContainer;
    public DeckCardEntryUI currentDeckEntryPrefab;

    [Header("Available Pool")]
    public Transform        poolContainer;
    public CardListEntryUI  poolEntryPrefab;

    [Header("Close")]
    public Button closeButton;

    List<DeckCardEntryUI>  spawnedCurrent = new();
    List<CardListEntryUI>  spawnedPool    = new();

    TeamRosterSlot  boundSlot;
    List<CardData>  allCards;

    void Awake()
    {
        closeButton?.onClick.AddListener(Close);
    }

    public void Open(TeamRosterSlot slot, List<CardData> cards)
    {
        boundSlot = slot;
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

        if (boundSlot == null || currentDeckEntryPrefab == null) return;

        foreach (var card in boundSlot.configuredDeck)
        {
            var entry = Instantiate(currentDeckEntryPrefab, currentDeckContainer);
            //interactable true so clicking removes the card
            entry.Setup(card, boundSlot, isInteractable: true);
            //override button to remove instead of open editor
            entry.button?.onClick.RemoveAllListeners();
            entry.button?.onClick.AddListener(() =>
                CombatPrepManager.Instance?.RemoveCard(boundSlot, card));
            spawnedCurrent.Add(entry);
        }
    }

    void RefreshPool()
    {
        foreach (var e in spawnedPool)
            if (e != null) Destroy(e.gameObject);
        spawnedPool.Clear();

        if (boundSlot == null || boundSlot.IsEmpty ||
            poolEntryPrefab == null) return;

        var pool = boundSlot.unit.GetFullCardPool(
            boundSlot.GetEffectiveKeypage());

        foreach (var card in pool)
        {
            var entry = Instantiate(poolEntryPrefab, poolContainer);
            bool inDeck = boundSlot.configuredDeck.Contains(card);
            entry.Setup(card, inDeck, boundSlot);
            spawnedPool.Add(entry);
        }
    }

    public void Close() => gameObject.SetActive(false);
}
