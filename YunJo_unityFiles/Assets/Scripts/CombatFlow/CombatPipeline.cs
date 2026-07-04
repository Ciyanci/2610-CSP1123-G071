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
    void Awake() => Instance = this;
    void CacheUnits()
    {
        allUnits.Clear();
        if (UnitRegistry.Instance == null) return;
        allUnits.AddRange(UnitRegistry.Instance.players);
        allUnits.AddRange(UnitRegistry.Instance.enemies);
    }
    //entry **
    public IEnumerator ResolveTurn()
    {
        Debug.Log("[PIPELINE] — TURN START");
        CacheUnits();
        CurrentPhase = CombatPipelinePhase.Resolve;
        foreach (var unit in allUnits)
        {
            if (unit == null || unit.IsDead) continue;
            unit.CommitAllSlots();
            Debug.Log($"[PIPELINE] Committed slots for {unit.unitName}");
        }
        ArrowManager.Instance?.ClearAllArrows();
        pageResolver?.ResetState();
        yield return BuildIntents();
        if (intents.Count == 0)
        {
            Debug.LogWarning("[PIPELINE] No intents — skipping resolution");
            yield return CleanupPhase();
            yield return CheckBattleEnd();
            yield break;
        }
        yield return ResolveIntents();
        yield return CleanupPhase();
        yield return CheckBattleEnd();
    }
    //build intents (1 to 1)
    IEnumerator BuildIntents()
    {
        intents.Clear();
        foreach (var u in allUnits)
        {
            if (u == null || u.IsDead) continue;
            int allowed = u.GetSpeedDiceCount();
            int added   = 0;
            foreach (var slot in u.speedSlots)
            {
                if (added >= allowed)
                {
                    Debug.Log($"[PIPELINE] {u.unitName} hit slot cap ({allowed}) — skipping remaining slots");
                    break;
                }
                if (slot == null)                          { Debug.Log($"[PIPELINE] {u.unitName} slot null — skip"); continue; }
                if (slot.state != SlotState.Committed)     { Debug.Log($"[PIPELINE] {u.unitName} slot not committed (state:{slot.state}) — skip"); continue; }
                if (slot.assignedCard == null)             { Debug.Log($"[PIPELINE] {u.unitName} slot has no card — skip"); continue; }
                if (slot.target == null)                   { Debug.Log($"[PIPELINE] {u.unitName} slot has no target — skip"); continue; }
                if (slot.target.IsDead)                    { Debug.Log($"[PIPELINE] {u.unitName} target already dead — skip"); continue; }
                var intent = new CombatIntent
                {
                    user      = u,
                    target    = slot.target,
                    speedSlot = slot,
                    card      = slot.assignedCard,
                    priority  = slot.value
                };
                intents.Add(intent);
                added++;
                Debug.Log($"[INTENT] [{added}/{allowed}] {u.unitName} → {slot.target.unitName} | spd:{slot.value} | card:{slot.assignedCard.Name}");
            }
        }
        intents = intents.OrderByDescending(i => i.priority).ToList();
        Debug.Log($"[PIPELINE] Built {intents.Count} total intent(s)");
        foreach (var i in intents)
            Debug.Log($"  {i.user.unitName} vs {i.target.unitName} | priority:{i.priority}");
        yield return null;
    }
    //resolve intents
    IEnumerator ResolveIntents()
    {
        var remaining = new List<CombatIntent>(intents);
        var resolved  = new HashSet<CombatIntent>();
        //play move sprite on all acting units — frozen-in-place illusion since htey dont move
        foreach (var intent in intents)
        {
            if (intent?.user == null || intent.user.IsDead) continue;
            if (intent.user.move != null)
                intent.user.sr.sprite = intent.user.move;
        }
        combatCamera?.SetCinematicView();
        yield return new WaitForSeconds(0.3f);
        int safetyCounter = 0;
        int maxIterations = intents.Count + 5;
        while (remaining.Count > 0 && safetyCounter < maxIterations)
        {
            safetyCounter++;
            //remove dead/invalid/already resolved
            remaining.RemoveAll(i =>
                i == null ||
                resolved.Contains(i) ||
                i.user == null ||
                i.user.IsDead);
            if (remaining.Count == 0) break;
            //pick highest priority intent
            var top = remaining[0];
            if (resolved.Contains(top))
            {
                remaining.RemoveAt(0);
                continue;
            }
            if (!top.IsValid)
            {
                Debug.Log($"[PIPELINE] Intent invalid at resolve time — skip: {top.user?.unitName}");
                resolved.Add(top);
                remaining.Remove(top);
                continue;
            }
            //find clash counter
            var counter = remaining.FirstOrDefault(i =>
                i != top &&
                !resolved.Contains(i) &&
                i.IsValid &&
                i.user   == top.target &&
                i.target == top.user   &&
                i.speedSlot?.state == SlotState.Committed);
            //dim uninvolved units
            SetInvolvedUnits(top, counter);
            if (counter != null)
            {
                Debug.Log($"[PIPELINE] CLASH: {top.user.unitName} vs {counter.user.unitName}");
                resolved.Add(top);
                resolved.Add(counter);
                remaining.Remove(top);
                remaining.Remove(counter);
                combatCamera?.FrameUnits(top.user, counter.user);
                yield return new WaitForSeconds(0.2f); // let camera ease in
                yield return ResolveClash(top, counter);
            }
            else
            {
                Debug.Log($"[PIPELINE] UNOPPOSED: {top.user.unitName} → {top.target.unitName}");
                resolved.Add(top);
                remaining.Remove(top);
                combatCamera?.FrameUnits(top.user, top.target);
                yield return new WaitForSeconds(0.2f);
                yield return ResolveUnopposed(top);
            }
            //restore transparency
            RestoreAllTransparency();
            //brief gap between resolutions
            yield return new WaitForSeconds(0.2f);
        }
        if (safetyCounter >= maxIterations)
            Debug.LogError("[PIPELINE] Safety counter hit — resolution loop may be stuck");
        Debug.Log("[PIPELINE] ========== RESOLUTION COMPLETE ==========");
    }
    //set involved units **
    void SetInvolvedUnits(CombatIntent a, CombatIntent b)
    {
        var involved = new HashSet<CharacterUnit>();
        if (a?.user   != null) involved.Add(a.user);
        if (a?.target != null) involved.Add(a.target);
        if (b?.user   != null) involved.Add(b.user);
        if (b?.target != null) involved.Add(b.target);
        foreach (var u in allUnits)
        {
            if (u == null || u.IsDead) continue;
            u.SetInvolved(involved.Contains(u));
        }
    }
    void RestoreAllTransparency()
    {
        foreach (var u in allUnits)
            if (u != null) u.ResetTransparency();
    }
    //unopposed **
    IEnumerator ResolveUnopposed(CombatIntent intent)
    {
        Debug.Log($"[RESOLVE] Unopposed: {intent.user.unitName} → {intent.target.unitName}");
        if (!intent.IsValid)
        {
            Debug.LogWarning("[RESOLVE] Unopposed aborted — intent invalid");
            yield break;
        }
        var page = intent.GetOrCreatePage();
        //start tracking aggressor + target so camera follows the charge
        combatCamera?.StartTracking(intent.user, intent.target);
        yield return pageResolver.ResolveSinglePage(page);
        //stop tracking once resolution is done
        combatCamera?.StopTracking();
        Debug.Log($"[RESOLVE] Unopposed complete: {intent.user.unitName}");
        FinalizeIntent(intent);
    }
    //clash **
    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        Debug.Log($"[RESOLVE] Clash start: {a.user.unitName}({a.card?.Name}) vs {b.user.unitName}({b.card?.Name})");
        if (!a.IsValid || !b.IsValid)
        {
            Debug.LogWarning($"[RESOLVE] Clash aborted — one or both intents invalid");
            yield break;
        }
        var pageA = a.GetOrCreatePage();
        var pageB = b.GetOrCreatePage();
        Debug.Log($"[RESOLVE] PageA: {pageA.dice.Count} dice | PageB: {pageB.dice.Count} dice");
        yield return pageResolver.ResolvePages(pageA, pageB);
        Debug.Log($"[RESOLVE] Clash complete: {a.user.unitName} vs {b.user.unitName}");
        FinalizeIntent(a);
        FinalizeIntent(b);
    }
    void FinalizeIntent(CombatIntent intent)
    {
        if (intent?.speedSlot == null) return;
        Debug.Log($"[FINALIZE] Clearing slot for {intent.user?.unitName}");
        intent.speedSlot.Clear();
        intent.ClearPage();
    }
    //cleanup **
    IEnumerator CleanupPhase()
    {
        CurrentPhase = CombatPipelinePhase.Cleanup;
        Debug.Log("[PHASE] Cleanup");
        ArrowManager.Instance?.ClearAllArrows();
        RestoreAllTransparency();
        // Restore idle sprites — units stay in their current positions
        foreach (var u in allUnits)
        {
            if (u == null || u.IsDead) continue;
            if (u.idle != null) u.sr.sprite = u.idle;
        }
        yield return new WaitForSeconds(0.3f);
    }
    //battle end **
    IEnumerator CheckBattleEnd()
    {
        bool playersAlive = UnitRegistry.Instance.players.Exists(p => p != null && !p.IsDead);
        bool enemiesAlive = UnitRegistry.Instance.enemies.Exists(e => e != null && !e.IsDead);
        if (!enemiesAlive)
        {
            Debug.Log("[PIPELINE] All enemies dead — VICTORY");
            yield return EndSequence();
            BattleResultsUI.Instance?.ShowVictory();
            yield break;
        }
        if (!playersAlive)
        {
            Debug.Log("[PIPELINE] All players dead — DEFEAT");
            yield return EndSequence();
            BattleResultsUI.Instance?.ShowDefeat();
            yield break;
        }
        yield return StartNextTurn();
    }
    IEnumerator EndSequence()
    {
        yield return TurnTransitionUI.Instance?.FadeToBlack();
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator StartNextTurn()
    {
        Debug.Log("[PIPELINE] Starting next turn");
        yield return TurnTransitionUI.Instance?.FadeToBlack();
        CinematicModeController.Instance?.ExitCinematic();
        pageResolver?.ResetState();
        foreach (var u in allUnits)
        {
            if (u == null || u.IsDead) continue;
            u.ResetPosition();
        }
        yield return new WaitForSeconds (1.5f);
        yield return TurnTransitionUI.Instance?.FadeFromBlack();
        Debug.Log("[PIPELINE] Resolution complete — notifying state machine");
        CombatStateMachine.Instance?.NotifyTurnComplete();
    }
    //camera ref for frozen-in-place move
    CombatCamera combatCamera =>
        pageResolver != null
            ? pageResolver.combatCamera
            : null;
}