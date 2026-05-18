using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public partial class CombatPipeline : MonoBehaviour
{
    public CombatPipelinePhase CurrentPhase;
    public static CombatPipeline Instance;
    [SerializeField] PageCombatResolver pageResolver;
    List<CombatIntent> intents = new();
    List<CharacterUnit> allUnits = new();
    void Awake()
    {
        Instance = this;
    }
    void CacheUnits()
    {
        allUnits.Clear();
        if (UnitRegistry.Instance == null)
            return;
        allUnits.AddRange(UnitRegistry.Instance.players);
        allUnits.AddRange(UnitRegistry.Instance.enemies);
    }
    //entry
    public IEnumerator ResolveTurn()
    {
        CinematicModeController.Instance?.EnterCinematic();
        CacheUnits();
        CurrentPhase = CombatPipelinePhase.Resolve;
        yield return BuildIntents();
        yield return ResolveIntents();
        yield return CleanupPhase();
        yield return CheckBattleEnd();
    }
    //build intents first
    IEnumerator BuildIntents()
    {
        intents.Clear();
        foreach (var u in allUnits)
        {
            if (u == null || u.IsDead)
                continue;
            foreach (var slot in u.speedSlots)
            {
                if (slot == null)
                    continue;
                if (slot.state != SlotState.Committed)
                    continue;
                if (slot.assignedCard == null || slot.target == null)
                    continue;
                intents.Add(new CombatIntent
                {
                    user     = u,
                    target   = slot.target,
                    speedSlot = slot,
                    card     = slot.assignedCard,
                    priority = slot.value
                });
            }
        }
        //speed priority (higher first)
        intents = intents
            .OrderByDescending(i => i.priority)
            .ToList();
        yield return null;
    }
    //resolve intents
    IEnumerator ResolveIntents()
    {
        HashSet<CombatIntent> resolved = new();
        foreach (var intent in intents)
        {
            if (intent == null || !intent.IsValid)
                continue;
            if (resolved.Contains(intent))
                continue;
            var counter = FindCounterIntent(intent);
            if (counter != null && !resolved.Contains(counter))
            {
                resolved.Add(intent);
                resolved.Add(counter);
                yield return ResolveClash(intent, counter);
            }
            else
            {
                resolved.Add(intent);
                yield return ResolveUnopposed(intent);
            }
        }
    }

    IEnumerator CheckBattleEnd()
    {
        bool playersAlive = UnitRegistry.Instance.players
            .Exists(p => p != null && !p.IsDead);
        bool enemiesAlive = UnitRegistry.Instance.enemies
            .Exists(e => e != null && !e.IsDead);

        if (!enemiesAlive)
        {
            CinematicModeController.Instance?.ExitCinematic();
            BattleResultsUI.Instance?.ShowVictory();
            yield break;
        }

        if (!playersAlive)
        {
            CinematicModeController.Instance?.ExitCinematic();
            BattleResultsUI.Instance?.ShowDefeat();
            yield break;
        }

        yield return StartNextTurn();
    }

    IEnumerator StartNextTurn()
    {
        CinematicModeController.Instance?.ExitCinematic();

        foreach (var u in allUnits)
        {
            if (u != null && !u.IsDead)
                u.ResetSpeedSlots();
                CombatHUDController.Instance?.ShowSpeedBubbles();
        }

        //refresh hands
        foreach (var p in UnitRegistry.Instance.players)
            p.deck?.RefreshHand();

        CombatFlowController.Instance.SetInputEnabled(true);
        CombatInfoBar.Instance?.ShowDefault();
        yield return null;
    }

    //unopposed (dice-by-dice damage applied now yay)
    IEnumerator ResolveUnopposed(CombatIntent intent)
    {
        if (!intent.IsValid)
            yield break;
        var page = intent.CreatePage();
        yield return pageResolver.ResolveSinglePage(page);
        FinalizeIntent(intent);
    }
    //clash (pagecombatresolver handles the clash loop now)
    // win - attacker deals damage, loser advances
    // draw - both advance (cancelled)
    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        if (!a.IsValid || !b.IsValid)
            yield break;
        Debug.Log($"[CLASH] {a.user.unitName} vs {b.user.unitName}");
        var pageA = a.CreatePage();
        var pageB = b.CreatePage();
        yield return pageResolver.ResolvePages(pageA, pageB);
        FinalizeIntent(a);
        FinalizeIntent(b);
    }
    //finalize intents (literally who cares if its finalise or finalize anymore bro)
    void FinalizeIntent(CombatIntent intent)
    {
        if (intent?.speedSlot == null)
            return;
        intent.speedSlot.Clear();
    }
    CombatIntent FindCounterIntent(CombatIntent intent)
    {
        return intents.FirstOrDefault(i =>
            i != intent &&
            i.user   == intent.target &&
            i.target == intent.user   &&
            i.speedSlot != null       &&
            i.speedSlot.state == SlotState.Committed
        );
    }
    //cleanup
    //ResetSpeedSlots() is the single authoritative reset (called in startturn)
    //CleanupPhase only needs to set the phase flag and pause
    IEnumerator CleanupPhase()
    {
        CurrentPhase = CombatPipelinePhase.Cleanup;
        Debug.Log("[PHASE] Cleanup");
        yield return new WaitForSeconds(0.3f);
    }
}