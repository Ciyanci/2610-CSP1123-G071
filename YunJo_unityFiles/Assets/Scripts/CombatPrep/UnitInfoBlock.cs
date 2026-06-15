using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
//use this for the panels in combat prep
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

    //set to true for player panel which enables remove button on deck cards
    public bool isInteractable = false;

    //overlay button covering the info section (player panel only again)
    public Button keypageOverlayButton;

    List<PassiveEntryUI>  spawnedPassives  = new();
    List<DeckCardEntryUI> spawnedDeckCards = new();

    TeamRosterSlot boundSlot;

    //for enemy units (no slot)
    public void BindEnemy(UnitData unit)
    {
        boundSlot = null;
        Populate(unit, null, isPlayer: false);
    }

    //for player slots
    public void BindSlot(TeamRosterSlot slot)
    {
        boundSlot = slot;
        if (slot == null || slot.IsEmpty)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        Populate(slot.unit, slot.GetEffectiveKeypage(), isPlayer: true);
    }

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
            speedRangeText.text = "SPD  1 ~ 9";  //static range, dice rolled in combat

        //resistances
        var res = unit.GetResistances(keypage);
        SetResText(slashText,  res.GetModifier(DamageType.Slash));
        SetResText(pierceText, res.GetModifier(DamageType.Pierce));
        SetResText(bluntText,  res.GetModifier(DamageType.Blunt));

        //keypage overlay button — player only, non-leader
        if (keypageOverlayButton != null)
        {
            keypageOverlayButton.gameObject.SetActive(isPlayer);
            keypageOverlayButton.onClick.RemoveAllListeners();
            if (isPlayer && boundSlot != null && !unit.isLeader)
                keypageOverlayButton.onClick.AddListener(() =>
                    CombatPrepManager.Instance?.OpenKeypageWindow(boundSlot));
        }

        RefreshPassives(unit, keypage);
        RefreshDeck();
    }

    public void RefreshDeck()
    {
        foreach (var e in spawnedDeckCards)
            if (e != null) Destroy(e.gameObject);
        spawnedDeckCards.Clear();

        if (deckCardPrefab == null) return;

        List<CardData> deck = boundSlot != null
            ? boundSlot.configuredDeck
            : new List<CardData>();

        foreach (var card in deck)
        {
            var entry = Instantiate(deckCardPrefab, deckContainer);
            entry.Setup(card, boundSlot, isInteractable);
            spawnedDeckCards.Add(entry);
        }
    }

    void RefreshPassives(UnitData unit, KeypageData keypage)
    {
        foreach (var e in spawnedPassives)
            if (e != null) Destroy(e.gameObject);
        spawnedPassives.Clear();

        if (passivePrefab == null) return;

        var passives = unit.GetActivePassives(keypage);
        foreach (var p in passives)
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
            >= 2.0f  => "Fatal",
            >= 1.5f  => "Weak",
            >= 1.0f  => "Normal",
            >= 0.5f  => "Endured",
            _        => "Ineffective"
        };
        label.color = mod switch
        {
            >= 2.0f  => new Color(0.9f, 0.2f, 0.2f),
            >= 1.5f  => new Color(0.9f, 0.6f, 0.2f),
            >= 1.0f  => Color.white,
            >= 0.5f  => new Color(0.4f, 0.8f, 0.4f),
            _        => new Color(0.4f, 0.6f, 0.9f)
        };
    }
}
