using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PageCombatResolver : MonoBehaviour
{
    [Header("Timing")]
    public float windupDuration     = 0.2f;
    public float hitPauseDuration   = 0.3f;
    public float diePauseDuration   = 0.5f;
    public float clashPauseDuration = 0.25f;
    [Header("Camera")]
    public CombatCamera combatCamera;
    public float clashShakeIntensity = 0.15f;
    public float hitShakeIntensity   = 0.08f;
    [Header("Dice Groups")]
    public CombatDiceGroupUI diceGroupLeft;
    public CombatDiceGroupUI diceGroupRight;

    HashSet<int> activePages = new();
    HashSet<int> completedPages = new();
    public void ResetState()
    {
        activePages.Clear();
        completedPages.Clear();

        diceGroupLeft?.Hide();
        diceGroupRight?.Hide();

        Debug.Log("[RESOLVER] State reset");
    }
    //clash entry**
    public IEnumerator ResolvePages(CombatPageRuntime a, CombatPageRuntime b)
    {
        if (activePages.Contains(a.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] A already resolving ({a.PageId})");

            yield break;
        }

        if (activePages.Contains(b.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] B already resolving ({b.PageId})");

            yield break;
        }

        if (completedPages.Contains(a.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] A already completed ({a.PageId})");

            yield break;
        }

        if (completedPages.Contains(b.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] B already completed ({b.PageId})");

            yield break;
        }

        activePages.Add(a.PageId);
        activePages.Add(b.PageId);
        bool aIsLeft = a.owner.transform.position.x < b.owner.transform.position.x;
        CharacterUnit leftUnit  = aIsLeft ? a.owner : b.owner;
        CharacterUnit rightUnit = aIsLeft ? b.owner : a.owner;
        Card leftCard           = aIsLeft ? a.card  : b.card;
        Card rightCard          = aIsLeft ? b.card  : a.card;
        CombatCardDisplayUI.Instance?.ShowForClash(leftUnit, leftCard, rightUnit, rightCard);
        diceGroupLeft?.Bind(leftUnit,  leftCard);
        diceGroupRight?.Bind(rightUnit, rightCard);
        CombatDiceGroupUI groupA = aIsLeft ? diceGroupLeft : diceGroupRight;
        CombatDiceGroupUI groupB = aIsLeft ? diceGroupRight : diceGroupLeft;
        Vector3 posA, posB;
        GetClashMeetPositions(a.owner, b.owner, out posA, out posB);
        Debug.Log($"[RESOLVER] Moving {a.owner.unitName} to {posA}, {b.owner.unitName} to {posB}");
        StartCoroutine(a.owner.MoveTo(posA, 0.3f, true, b.owner));
        StartCoroutine(b.owner.MoveTo(posB, 0.3f, true, a.owner));
        yield return new WaitForSeconds(0.3f);
        FaceUnitsTowardEachOther(a.owner, b.owner);
        Vector3 clashPosA = a.owner.visual.position;
        Vector3 clashPosB = b.owner.visual.position;
        //clash loop**
        int clashIteration = 0;
        int maxClash        = a.dice.Count + b.dice.Count + 2;
        while (!a.IsFinished && !b.IsFinished && clashIteration < maxClash)
        {
            clashIteration++;
            if (!a.owner.CanResolveAction() || !b.owner.CanResolveAction())
            {
                Debug.Log("[RESOLVER] One unit cannot resolve — breaking clash loop");
                break;
            }
            var dieA = a.GetCurrentDie();
            var dieB = b.GetCurrentDie();
            if (dieA == null || dieB == null)
            {
                Debug.Log("[RESOLVER] Null die — breaking clash loop");
                activePages.Remove(a.PageId);
                activePages.Remove(b.PageId);
                break;
            }
            Debug.Log($"[RESOLVER] Clash iteration {clashIteration}: {a.owner.unitName}[{a.currentIndex}/{a.dice.Count}] vs {b.owner.unitName}[{b.currentIndex}/{b.dice.Count}]");
            yield return ReturnToPositions(a.owner, clashPosA, b.owner, clashPosB);
            FaceUnitsTowardEachOther(a.owner, b.owner);
            a.owner.PlayWindup();
            b.owner.PlayWindup();
            int rollA = 0, rollB = 0;
            bool doneA = false, doneB = false;
            StartCoroutine(groupA != null
                ? groupA.RollCurrentDie(dieA.data.minRoll, dieA.data.maxRoll,
                    r => { rollA = r; doneA = true; })
                : DoneImmediate(dieA.Roll(), r => { rollA = r; doneA = true; }));
            StartCoroutine(groupB != null
                ? groupB.RollCurrentDie(dieB.data.minRoll, dieB.data.maxRoll,
                    r => { rollB = r; doneB = true; })
                : DoneImmediate(dieB.Roll(), r => { rollB = r; doneB = true; }));
            yield return new WaitUntil(() => doneA && doneB);
            dieA.roll = rollA;
            dieB.roll = rollB;
            Debug.Log($"[RESOLVER] Rolls: {a.owner.unitName}={rollA} vs {b.owner.unitName}={rollB}");
            if      (rollA > rollB) { groupA?.SetCurrentResult(true);  groupB?.SetCurrentResult(false); }
            else if (rollB > rollA) { groupB?.SetCurrentResult(true);  groupA?.SetCurrentResult(false); }
            else                   { groupA?.SetCurrentResult(false); groupB?.SetCurrentResult(false); }
            yield return new WaitForSeconds(clashPauseDuration);
            if (rollA > rollB)
            {
                groupB?.BreakCurrentDie();
                groupA?.AdvanceDie();
                groupB?.AdvanceDie();
                yield return ApplyClashHit(a.owner, b.owner, dieA, rollA);
                b.Advance();
                clashPosB = b.owner.visual.position;
            }
            else if (rollB > rollA)
            {
                groupA?.BreakCurrentDie();
                groupA?.AdvanceDie();
                groupB?.AdvanceDie();
                yield return ApplyClashHit(b.owner, a.owner, dieB, rollB);
                a.Advance();
                clashPosA = a.owner.visual.position;
            }
            else
            {
                Debug.Log($"[RESOLVER] Draw — both dice cancelled");
                groupA?.BreakCurrentDie();
                groupB?.BreakCurrentDie();
                groupA?.AdvanceDie();
                groupB?.AdvanceDie();
                a.owner.sr.sprite = a.owner.idle;
                b.owner.sr.sprite = b.owner.idle;
                a.Advance();
                b.Advance();
            }
            yield return new WaitForSeconds(diePauseDuration);
        }
        
        Debug.Log($"[RESOLVER] Clash loop done. A remaining:{a.dice.Count - a.currentIndex} B remaining:{b.dice.Count - b.currentIndex}");
        if (!a.IsFinished && a.owner.CanResolveAction() && !b.owner.IsDead)
        {
            Debug.Log($"[RESOLVER] Flushing remaining dice for {a.owner.unitName}");
            yield return RunRemainingDice(a, b.owner, groupA);
        }
        if (!b.IsFinished && b.owner.CanResolveAction() && !a.owner.IsDead)
        {
            Debug.Log($"[RESOLVER] Flushing remaining dice for {b.owner.unitName}");
            yield return RunRemainingDice(b, a.owner, groupB);
        }
        completedPages.Add(a.PageId);
        completedPages.Add(b.PageId);
        activePages.Remove(a.PageId);
        activePages.Remove(b.PageId);
        Debug.Log($"[RESOLVER] ResolvePages complete: {a.owner.unitName} vs {b.owner.unitName}");
        diceGroupLeft?.Hide();
        diceGroupRight?.Hide();
        CombatCardDisplayUI.Instance?.Hide();
    }
    //unopposed**
    public IEnumerator ResolveSinglePage(CombatPageRuntime page)
    {
        Debug.Log($"[RESOLVER] ResolveSinglePage called: {page?.owner?.unitName} ({page?.dice.Count} dice)");
        if (page == null)
        {
            Debug.LogWarning("[RESOLVER] ResolveSinglePage — page is null");
            yield break;
        }
        if (activePages.Contains(page.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] Page already resolving ({page.PageId})");

            yield break;
        }

        if (completedPages.Contains(page.PageId))
        {
            Debug.LogWarning(
                $"[RESOLVER] Page already completed ({page.PageId})");

            yield break;
        }

        activePages.Add(page.PageId);
        if (page.owner == null)
        {
            Debug.LogWarning("[RESOLVER] ResolveSinglePage — page.owner is null");

            activePages.Remove(page.PageId);

            yield break;
        }
        CombatCardDisplayUI.Instance?.ShowForUnopposed(page.owner, page.card);
        bool attackerIsLeft = page.target != null &&
            page.owner.transform.position.x < page.target.transform.position.x;
        CombatDiceGroupUI attackerGroup = attackerIsLeft ? diceGroupLeft : diceGroupRight;
        CombatDiceGroupUI otherGroup = attackerIsLeft ? diceGroupRight : diceGroupLeft;

        otherGroup?.Hide();
        attackerGroup?.Bind(page.owner, page.card);

        yield return RunRemainingDice(page, page.target, attackerGroup);

        completedPages.Add(page.PageId);
        activePages.Remove(page.PageId);

        Debug.Log($"[RESOLVER] ResolveSinglePage complete: {page.owner.unitName}");

        attackerGroup?.Hide();
        CombatCardDisplayUI.Instance?.Hide();
    }

    //run remaining dice**
    IEnumerator RunRemainingDice(
        CombatPageRuntime page,
        CharacterUnit target,
        CombatDiceGroupUI group)
    {
        Debug.Log($"[RESOLVER] RunRemainingDice: {page.owner.unitName} dice[{page.currentIndex}..{page.dice.Count - 1}] → {target?.unitName}");
        while (!page.IsFinished)
        {
            if (!page.owner.CanResolveAction())
            {
                Debug.Log($"[RESOLVER] RunRemainingDice — {page.owner.unitName} cannot resolve");
                break;
            }
            if (target == null || target.IsDead)
            {
                Debug.Log($"[RESOLVER] RunRemainingDice — target dead");
                break;
            }
            var die = page.GetCurrentDie();
            if (die == null)
            {
                Debug.Log($"[RESOLVER] RunRemainingDice — null die at index {page.currentIndex}, advancing");
                page.Advance();
                group?.AdvanceDie();
                continue;
            }
            Debug.Log($"[RESOLVER] RunRemainingDice die {page.currentIndex + 1}/{page.dice.Count}: {page.owner.unitName} → {target.unitName}");
            Vector3 attackPos = target.clashAnchor != null
                ? target.clashAnchor.position
                : GetApproachPosition(page.owner, target);
            yield return page.owner.MoveTo(attackPos, 0.22f, true, target);
            FaceUnitsTowardEachOther(page.owner, target);
            yield return ApplyFlushHit(page.owner, target, die, group);
            page.Advance();
            group?.AdvanceDie();
            Debug.Log($"[RESOLVER] Die resolved. page.currentIndex now: {page.currentIndex}");
            yield return new WaitForSeconds(diePauseDuration);
        }
        Debug.Log($"[RESOLVER] RunRemainingDice complete: {page.owner.unitName} finished={page.IsFinished}");
    }
    //clash hit **
    IEnumerator ApplyClashHit(
        CharacterUnit attacker,
        CharacterUnit defender,
        PageDie die,
        int roll)
    {
        attacker.PlayAttack();
        defender.PlayHit();
        CombatAudioManager.Instance?.PlayClashHit();

        int damage  = Mathf.Max(1, roll + die.Power);
        Vector3 dir = (defender.visual.position - attacker.visual.position).normalized;

        yield return defender.TakeDamageWithKnockback(damage, die.damageType, dir, false);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));

        yield return CameraShake(clashShakeIntensity, 0.2f);
        yield return new WaitForSeconds(hitPauseDuration);

        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;
    }

    //flush hit **
    IEnumerator ApplyFlushHit(
        CharacterUnit attacker,
        CharacterUnit defender,
        PageDie die,
        CombatDiceGroupUI group)
    {
        attacker.PlayWindup();

        int roll = 0;
        if (group != null)
            yield return group.RollCurrentDie(
                die.data.minRoll, die.data.maxRoll, r => roll = r);
        else
        {
            yield return new WaitForSeconds(windupDuration);
            roll = die.Roll();
        }

        group?.SetCurrentResult(true);
        yield return new WaitForSeconds(0.15f);

        attacker.PlayAttack();
        defender.PlayHit();
        CombatAudioManager.Instance?.PlayUnopposedHit();

        int damage  = Mathf.Max(1, roll + die.Power);
        Vector3 dir = (defender.visual.position - attacker.visual.position).normalized;

        yield return defender.TakeDamageWithKnockback(damage, die.damageType, dir, false);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));

        yield return CameraShake(hitShakeIntensity, 0.15f);
        yield return new WaitForSeconds(hitPauseDuration);

        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;
    }

    //return to clash positions**
    IEnumerator ReturnToPositions(
        CharacterUnit a, Vector3 posA,
        CharacterUnit b, Vector3 posB)
    {
        bool aClose = Vector3.Distance(a.visual.position, posA) < 0.1f;
        bool bClose = Vector3.Distance(b.visual.position, posB) < 0.1f;
        if (aClose && bClose) yield break;

        float dur = 0.18f;
        if (!aClose) StartCoroutine(a.MoveTo(posA, dur, true, b));
        if (!bClose) StartCoroutine(b.MoveTo(posB, dur, true, a));
        yield return new WaitForSeconds(dur);
    }

    //approach fallback
    Vector3 GetApproachPosition(CharacterUnit attacker, CharacterUnit target)
    {
        if (target == null) return attacker.visual.position;
        bool isLeft = attacker.transform.position.x < target.visual.position.x;
        float sign  = isLeft ? -1f : 1f;
        float offset = ClashLane.Instance != null
            ? ClashLane.Instance.attackNearOffset : 5f;
        return target.visual.position + new Vector3(sign * offset, 0f, 0f);
    }

    //facing helper ** (clashing needs tweaking)
    void FaceUnitsTowardEachOther(CharacterUnit a, CharacterUnit b)
    {
        if (a == null || b == null) return;
        a.FaceTowardUnit(b);
        b.FaceTowardUnit(a);
    }

    //helpers **
    IEnumerator CameraShake(float intensity, float duration)
    {
        if (combatCamera != null)
            yield return combatCamera.Shake(intensity, duration);
    }

    IEnumerator DoneImmediate(int val, System.Action<int> cb)
    {
        cb?.Invoke(val);
        yield break;
    }

    void GetClashMeetPositions(
        CharacterUnit a,
        CharacterUnit b,
        out Vector3 posA,
        out Vector3 posB)
    {
        Vector3 anchorA = a.clashAnchor != null ? a.clashAnchor.position : a.visual.position;
        Vector3 anchorB = b.clashAnchor != null ? b.clashAnchor.position : b.visual.position;
        Vector3 mid = (anchorA + anchorB) * 0.5f;
        //offset each unit to their side of the midpoint
        float separation = ClashLane.Instance != null
            ? ClashLane.Instance.clashStandOffset
            : 2f;
        bool aIsLeft = a.transform.position.x < b.transform.position.x;
        posA = mid + new Vector3(aIsLeft ? -separation : separation, 0f, 0f);
        posB = mid + new Vector3(aIsLeft ?  separation : -separation, 0f, 0f);
    }
}
