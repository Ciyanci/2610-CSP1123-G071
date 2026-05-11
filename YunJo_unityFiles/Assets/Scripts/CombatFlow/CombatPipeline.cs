using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public partial class CombatPipeline : MonoBehaviour
{
    public CombatPipelinePhase CurrentPhase;
    public static CombatPipeline Instance;
    // FIX #2/#3: wire PageCombatResolver in the Inspector
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
    // =========================
    // ENTRY
    // =========================
    public IEnumerator ResolveTurn()
    {
        CacheUnits();
        CurrentPhase = CombatPipelinePhase.Resolve;
        yield return BuildIntents();
        yield return ResolveIntents();
        yield return CleanupPhase();
    }
    // =========================
    // 1. BUILD INTENTS
    // =========================
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
        // Highest speed acts first
        intents = intents
            .OrderByDescending(i => i.priority)
            .ToList();
        yield return null;
    }
    // =========================
    // 2. RESOLVE INTENTS
    // =========================
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
    // =========================
    // UNOPPOSED
    // FIX #3: actually apply damage die-by-die
    // =========================
    IEnumerator ResolveUnopposed(CombatIntent intent)
    {
        if (!intent.IsValid)
            yield break;
        var page = intent.CreatePage();
        yield return pageResolver.ResolveSinglePage(page);
        FinalizeIntent(intent);
    }
    // =========================
    // CLASH
    // FIX #2: delegate to PageCombatResolver which actually rolls and damages
    // =========================
    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        if (!a.IsValid || !b.IsValid)
            yield break;
        Debug.Log($"[CLASH] {a.user.unitName} vs {b.user.unitName}");
        var pageA = a.CreatePage();
        var pageB = b.CreatePage();
        // PageCombatResolver handles the full clash loop:
        // win → attacker deals damage, loser advances
        // draw → both advance (cancelled)
        // then flushes any remaining dice unopposed
        yield return pageResolver.ResolvePages(pageA, pageB);
        FinalizeIntent(a);
        FinalizeIntent(b);
    }
    // =========================
    // FINALIZE
    // =========================
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
    // =========================
    // CLEANUP
    // FIX #9: removed redundant slot.Clear() loop.
    // StartTurn → ResetSpeedSlots() is the single authoritative reset.
    // CleanupPhase only needs to set the phase flag and pause.
    // =========================
    IEnumerator CleanupPhase()
    {
        CurrentPhase = CombatPipelinePhase.Cleanup;
        Debug.Log("[PHASE] Cleanup");
        yield return new WaitForSeconds(0.3f);
    }
}