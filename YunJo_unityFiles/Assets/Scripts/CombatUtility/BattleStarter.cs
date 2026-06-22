using UnityEngine;
using System.Collections;

public class BattleStarter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        UnitRegistry.Instance.Refresh();
        yield return null; // let all Awake/Start finish

        //apply unitdata to characters on scene
        //this must happen before Init() so decks have cards
        foreach (var unit in UnitRegistry.Instance.players)
            ApplyUnitData(unit);

        foreach (var unit in UnitRegistry.Instance.enemies)
            ApplyUnitData(unit);

        //init decks AFTER UnitData has populated startingDeck
        foreach (var unit in UnitRegistry.Instance.players)
            unit.deck?.Init();

        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.deck?.Init();

        CombatHUDController.Instance?.Bind();
        CombatAudioManager.Instance?.PlayTurnBegin();

        Debug.Log("[BATTLE] Battle started");
    }

    void ApplyUnitData(CharacterUnit unit)
    {
        if (unit == null) return;

        UnitData    data    = unit.unitData;
        KeypageData keypage = unit.equippedKeypage;

        if (data == null)
        {
            Debug.LogWarning($"[BATTLE] {unit.unitName} has no UnitData assigned");
            return;
        }

        //apply stats from UnitData + keypage
        unit.unitName   = data.unitName;
        unit.maxHP      = data.GetMaxHP(keypage);
        unit.hp         = unit.maxHP;
        unit.maxStagger = data.GetMaxStagger(keypage);
        unit.stagger    = unit.maxStagger;
        unit.maxLight   = data.maxLight;
        unit.currentLight = data.maxLight;
        unit.resistances  = data.GetResistances(keypage);

        //apply sprites if UnitData has them
        if (data.idleSprite   != null) unit.idle   = data.idleSprite;
        if (data.attackSprite != null) unit.attack = data.attackSprite;
        if (data.hitSprite    != null) unit.hit    = data.hitSprite;
        if (data.windupSprite != null) unit.windup = data.windupSprite;
        if (data.moveSprite   != null) unit.move   = data.moveSprite;

        if (unit.sr != null && unit.idle != null)
            unit.sr.sprite = unit.idle;

        //copy card pool into CharacterDeck.startingDeck so Init() has something to build from
        if (unit.deck != null)
        {
            unit.deck.startingDeck.Clear();

            var pool = data.GetFullCardPool(keypage);
            foreach (var card in pool)
                unit.deck.startingDeck.Add(card);

            Debug.Log($"[BATTLE] {unit.unitName} deck loaded: " +
                      $"{unit.deck.startingDeck.Count} cards");
        }

        Debug.Log($"[BATTLE] Applied {data.unitName} | " +
                  $"HP:{unit.maxHP} STG:{unit.maxStagger} " +
                  $"Keypage:{keypage?.keypageName ?? "none"}");

        Debug.Log($"SPEED SLOTS APPLIED {data.unitName}");
        unit.InitializeSpeedSlots();
        if (unit.slotRowUI != null)
            unit.slotRowUI.Bind(unit);
    }
}
