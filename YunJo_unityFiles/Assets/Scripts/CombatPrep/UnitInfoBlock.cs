using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UnitInfoBlock : MonoBehaviour
{
    [Header("Identity")]
    public Image           portrait;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI keypageNameText;

    [Header("Stats")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI staggerText;
    public TextMeshProUGUI speedRangeText;

    [Header("Resistances")]
    public TextMeshProUGUI slashText;
    public TextMeshProUGUI pierceText;
    public TextMeshProUGUI bluntText;

    [Header("Passives")]
    public Transform      passiveContainer;
    public PassiveEntryUI passivePrefab;

    [Header("Deck")]
    public Transform       deckContainer;
    public DeckCardEntryUI deckCardPrefab;

    [Header("Interaction")]
    public bool   isInteractable = false;
    public Button keypageOverlayButton;

    List<PassiveEntryUI>  spawnedPassives  = new();
    List<DeckCardEntryUI> spawnedDeckCards = new();

    CharacterUnit boundUnit;

    //bind (enemy is read only)
    public void BindEnemy(UnitData unit)
    {
        boundUnit = null;

        if (unit == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        keypageOverlayButton?.gameObject.SetActive(false);

        Populate(unit, null, isPlayer: false);
    }

    //bind (player)
    public void BindUnit(CharacterUnit unit)
    {
        boundUnit = unit;

        if (unit == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        UnitData    data    = unit.unitData;
        KeypageData keypage = unit.equippedKeypage;

        //keypage button — visible for non-leader players only
        if (keypageOverlayButton != null)
        {
            bool canChangeKeypage = isInteractable &&
                                    data != null &&
                                    !data.isLeader;

            keypageOverlayButton.gameObject.SetActive(canChangeKeypage);
            keypageOverlayButton.onClick.RemoveAllListeners();

            if (canChangeKeypage)
                keypageOverlayButton.onClick.AddListener(() =>
                    CombatPrepManager.Instance?.OpenKeypageWindow(unit));
        }

        Populate(data, keypage, isPlayer: true);
    }

    //refresh (call after deck/keypage changes)
    public void RefreshDeck()
    {
        if (boundUnit != null)
            SpawnDeckCards(boundUnit.deck?.startingDeck, boundUnit);
    }

    //internal
    void Populate(UnitData unit, KeypageData keypage, bool isPlayer)
    {
        if (unit == null) return;

        if (portrait      != null && unit.portrait != null)
            portrait.sprite   = unit.portrait;

        if (unitNameText  != null)
            unitNameText.text  = unit.unitName;

        if (keypageNameText != null)
            keypageNameText.text = keypage != null
                ? keypage.keypageName : "No Keypage";

        if (hpText      != null)
            hpText.text      = $"HP  {unit.GetMaxHP(keypage)}";

        if (staggerText != null)
            staggerText.text = $"STG  {unit.GetMaxStagger(keypage)}";

        if (speedRangeText != null)
            speedRangeText.text = "SPD  1 – 9";

        var res = unit.GetResistances(keypage);
        SetResText(slashText,  res.GetModifier(DamageType.Slash));
        SetResText(pierceText, res.GetModifier(DamageType.Pierce));
        SetResText(bluntText,  res.GetModifier(DamageType.Blunt));

        RefreshPassives(unit, keypage);

        //deck (only player has live characterunit display)
        if (isPlayer && boundUnit != null)
            SpawnDeckCards(boundUnit.deck?.startingDeck, boundUnit);
        else
            SpawnDeckCards(null, null);
    }

    void SpawnDeckCards(List<CardData> deck, CharacterUnit unit)
    {
        foreach (var e in spawnedDeckCards)
            if (e != null) Destroy(e.gameObject);
        spawnedDeckCards.Clear();

        if (deckCardPrefab == null || deck == null) return;

        foreach (var card in deck)
        {
            var entry = Instantiate(deckCardPrefab, deckContainer);
            entry.Setup(card, unit, isInteractable);
            spawnedDeckCards.Add(entry);
        }
    }

    void RefreshPassives(UnitData unit, KeypageData keypage)
    {
        foreach (var e in spawnedPassives)
            if (e != null) Destroy(e.gameObject);
        spawnedPassives.Clear();

        if (passivePrefab == null || unit == null) return;

        foreach (var p in unit.GetActivePassives(keypage))
        {
            var entry = Instantiate(passivePrefab, passiveContainer);
            entry.Setup(p);
            spawnedPassives.Add(entry);
        }
    }

    void SetResText(TextMeshProUGUI label, float mod)
    {
        if (label == null) return;
        label.text = mod switch
        {
            >= 2.0f => "Fatal",
            >= 1.5f => "Weak",
            >= 1.0f => "Normal",
            >= 0.5f => "Endured",
            _       => "Ineffective"
        };
        label.color = mod switch
        {
            >= 2.0f => new Color(0.9f, 0.2f, 0.2f),
            >= 1.5f => new Color(0.9f, 0.6f, 0.2f),
            >= 1.0f => Color.white,
            >= 0.5f => new Color(0.4f, 0.8f, 0.4f),
            _       => new Color(0.4f, 0.6f, 0.9f)
        };
    }
}
