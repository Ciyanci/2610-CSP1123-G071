using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleStarter : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> playerSpawnPoints = new();
    public List<Transform> enemySpawnPoints  = new();

    [Header("Fallback — used if no BattleContext is set (editor testing)")]
    public TeamRoster  fallbackRoster;
    public StageData   fallbackStage;

    void Start()
    {
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        yield return null; // let awake finish

        StageData  stage  = BattleContext.ActiveStage  ?? fallbackStage;
        TeamRoster roster = BattleContext.ActiveRoster ?? fallbackRoster;

        if (stage == null || roster == null)
        {
            Debug.LogError("[BATTLE] No stage or roster — assign fallbacks in Inspector");
            yield break;
        }

        //spawn and configure player units
        var filledSlots = roster.GetFilledSlots();
        for (int i = 0; i < filledSlots.Count; i++)
        {
            var slot = filledSlots[i];
            if (slot.IsEmpty) continue;

            Transform spawnPoint = i < playerSpawnPoints.Count
                ? playerSpawnPoints[i]
                : null;

            SpawnPlayerUnit(slot, spawnPoint, i);
        }

        //spawn enemy units from StageData
        for (int i = 0; i < stage.enemyUnits.Count; i++)
        {
            UnitData enemyData = stage.enemyUnits[i];
            if (enemyData == null) continue;

            Transform spawnPoint = i < enemySpawnPoints.Count
                ? enemySpawnPoints[i]
                : null;

            SpawnEnemyUnit(enemyData, spawnPoint, i);
        }

        //refresh registry after spawning
        UnitRegistry.Instance.Refresh();

        CombatHUDController.Instance?.Bind();
        CombatAudioManager.Instance?.PlayTurnBegin();

        Debug.Log("[BATTLE] Battle started");

        BattleContext.Clear();
    }

    void SpawnPlayerUnit(TeamRosterSlot slot, Transform spawnPoint, int index)
    {
        if (slot.unit == null || slot.unit.idleSprite == null)
        {
            Debug.LogWarning($"[BATTLE] Player slot {index} has no unit or sprite");
            return;
        }

        //reuse pre-placed player GameObjects if spawn points are scene objects with CharacterUnit already attached, otherwise instantiate
        CharacterUnit unit = spawnPoint != null
            ? spawnPoint.GetComponent<CharacterUnit>()
            : null;

        if (unit == null)
        {
            Debug.LogWarning($"[BATTLE] No CharacterUnit at player spawn point {index}");
            return;
        }

        ApplyUnitData(unit, slot, isEnemy: false);
    }

    void SpawnEnemyUnit(UnitData data, Transform spawnPoint, int index)
    {
        if (data == null) return;

        StageData stage = BattleContext.ActiveStage ?? fallbackStage;

        if (stage.enemyPrefab == null)
        {
            Debug.LogWarning("[BATTLE] StageData.enemyPrefab is not assigned");
            return;
        }

        Vector3 pos = spawnPoint != null
            ? spawnPoint.position
            : Vector3.right * (index * 5f);

        GameObject go = Instantiate(stage.enemyPrefab, pos, Quaternion.identity);
        go.tag = "Enemy";

        CharacterUnit unit = go.GetComponent<CharacterUnit>();
        if (unit == null)
        {
            Debug.LogWarning("[BATTLE] enemyPrefab has no CharacterUnit");
            return;
        }

        //enemies use no keypage so base stats only
        var slot = new TeamRosterSlot { unit = data };
        slot.InitializeDeck();

        ApplyUnitData(unit, slot, isEnemy: true);
    }

    void ApplyUnitData(CharacterUnit unit, TeamRosterSlot slot, bool isEnemy)
    {
        UnitData     data    = slot.unit;
        KeypageData  keypage = slot.GetEffectiveKeypage();

        //identity
        unit.unitName = data.unitName;

        //stats
        unit.maxHP      = data.GetMaxHP(keypage);
        unit.hp         = unit.maxHP;
        unit.maxStagger = data.GetMaxStagger(keypage);
        unit.stagger    = unit.maxStagger;
        unit.maxLight   = data.maxLight;
        unit.currentLight = data.maxLight;

        //resistances
        unit.resistances = data.GetResistances(keypage);

        //sprites
        unit.idle   = data.idleSprite;
        unit.attack = data.attackSprite;
        unit.hit    = data.hitSprite;
        unit.windup = data.windupSprite;
        unit.move   = data.moveSprite;

        if (unit.sr != null && data.idleSprite != null)
            unit.sr.sprite = data.idleSprite;

        //deck
        if (unit.deck != null)
        {
            unit.deck.LoadFromCardList(slot.configuredDeck);
        }

        Debug.Log($"[BATTLE] Applied {data.unitName} " +
                  $"HP:{unit.maxHP} STG:{unit.maxStagger} " +
                  $"Keypage:{keypage?.keypageName ?? "none"}");
    }
}
