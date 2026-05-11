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
    [Header("Dice UI")]
    public DiceUI diceUILeft;    // attacker / side A
    public DiceUI diceUIRight;   // defender / side B
    // =========================
    // CLASH ENTRY
    // =========================
    public IEnumerator ResolvePages(CombatPageRuntime a, CombatPageRuntime b)
    {
        // Frame both units
        yield return CameraAction(CameraActionType.FrameTargets,
            targets: new List<Transform> { a.owner.visual, b.owner.visual },
            duration: 0.3f);
        diceUILeft?.Follow(a.owner.headAnchor);
        diceUIRight?.Follow(b.owner.headAnchor);
        // =========================
        // CLASH LOOP
        // =========================
        while (!a.IsFinished && !b.IsFinished)
        {
            if (!a.owner.CanResolveAction() || !b.owner.CanResolveAction())
                break;
            var dieA = a.GetCurrentDie();
            var dieB = b.GetCurrentDie();
            if (dieA == null || dieB == null) break;
            // Wind-up
            a.owner.PlayWindup();
            b.owner.PlayWindup();
            yield return new WaitForSeconds(windupDuration);
            // Roll both dice with animated UI
            int rollA = 0, rollB = 0;
            yield return RollBoth(dieA, dieB,
                r => rollA = r,
                r => rollB = r);
            // Colour the result
            diceUILeft?.SetResult(rollA, rollB);
            diceUIRight?.SetResult(rollB, rollA);
            yield return new WaitForSeconds(clashPauseDuration);
            // Resolve outcome
            if (rollA > rollB)
            {
                yield return ApplyClashHit(a.owner, b.owner, dieA, rollA);
                b.Advance();    // loser die cancelled
            }
            else if (rollB > rollA)
            {
                yield return ApplyClashHit(b.owner, a.owner, dieB, rollB);
                a.Advance();    // loser die cancelled
            }
            else
            {
                Debug.Log("[CLASH] DRAW — both dice cancelled");
                a.owner.sr.sprite = a.owner.idle;
                b.owner.sr.sprite = b.owner.idle;
                a.Advance();
                b.Advance();
            }
            yield return new WaitForSeconds(diePauseDuration);
        }
        // =========================
        // FLUSH remaining dice
        // =========================
        yield return FlushRemaining(a, b.owner);
        yield return FlushRemaining(b, a.owner);
        diceUILeft?.Hide();
        diceUIRight?.Hide();
        yield return CameraAction(CameraActionType.Reset, duration: 0.3f);
    }
    // =========================
    // FLUSH — unopposed tail after clash ends
    // =========================
    IEnumerator FlushRemaining(CombatPageRuntime page, CharacterUnit target)
    {
        while (!page.IsFinished)
        {
            if (!page.owner.CanResolveAction()) yield break;
            if (target == null || target.IsDead) yield break;
            var die = page.GetCurrentDie();
            if (die == null) { page.Advance(); continue; }
            yield return ApplyUnopposedHit(page.owner, target, die);
            page.Advance();
            yield return new WaitForSeconds(diePauseDuration);
        }
    }
    // =========================
    // SINGLE UNOPPOSED PAGE
    // Called from CombatPipeline.ResolveUnopposed
    // =========================
    public IEnumerator ResolveSinglePage(CombatPageRuntime page)
    {
        if (page?.owner == null) yield break;
        yield return CameraAction(CameraActionType.FocusTarget,
            target: page.owner.visual,
            duration: 0.25f);
        diceUILeft?.Follow(page.owner.headAnchor);
        while (!page.IsFinished)
        {
            if (!page.owner.CanResolveAction()) yield break;
            if (page.target == null || page.target.IsDead) yield break;
            var die = page.GetCurrentDie();
            if (die == null) { page.Advance(); continue; }
            yield return ApplyUnopposedHit(page.owner, page.target, die);
            page.Advance();
            yield return new WaitForSeconds(diePauseDuration);
        }
        diceUILeft?.Hide();
        yield return CameraAction(CameraActionType.Reset, duration: 0.25f);
    }
    // =========================
    // CLASH HIT
    // =========================
    IEnumerator ApplyClashHit(
        CharacterUnit attacker,
        CharacterUnit defender,
        PageDie die,
        int roll)
    {
        // Lunge toward defender's clash anchor
        if (attacker.clashAnchor != null && defender.clashAnchor != null)
            yield return attacker.MoveTo(defender.clashAnchor.position, 0.12f);
        attacker.PlayAttack();
        defender.PlayHit();
        int damage = Mathf.Max(1, roll + die.Power);
        defender.TakeDamage(damage, die.damageType);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));
        Debug.Log($"[CLASH HIT] {attacker.unitName} → {defender.unitName} " +
                  $"| {die.damageType} {damage} HP (roll {roll} + pow {die.Power})");
        yield return CameraAction(CameraActionType.Shake,
            shakeIntensity: clashShakeIntensity,
            duration: 0.2f);
        yield return new WaitForSeconds(hitPauseDuration);
        attacker.ResetPosition();
        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;
    }
    // =========================
    // UNOPPOSED HIT
    // =========================
    IEnumerator ApplyUnopposedHit(
        CharacterUnit attacker,
        CharacterUnit defender,
        PageDie die)
    {
        attacker.PlayWindup();
        yield return new WaitForSeconds(windupDuration);
        // Animate the roll on diceUILeft
        int roll = 0;
        if (diceUILeft != null)
            yield return diceUILeft.Roll(die.data.minRoll, die.data.maxRoll, r => roll = r);
        else
            roll = die.Roll();
        attacker.PlayAttack();
        defender.PlayHit();
        int damage = Mathf.Max(1, roll + die.Power);
        defender.TakeDamage(damage, die.damageType);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));
        Debug.Log($"[UNOPPOSED] {attacker.unitName} → {defender.unitName} " +
                  $"| {die.damageType} {damage} HP (roll {roll} + pow {die.Power})");
        yield return CameraAction(CameraActionType.Shake,
            shakeIntensity: hitShakeIntensity,
            duration: 0.15f);
        yield return new WaitForSeconds(hitPauseDuration);
        attacker.ResetPosition();
        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;
    }
    // =========================
    // ROLL BOTH — simultaneous animated dice
    // Uses lastRoll from DiceUI as the authoritative result
    // =========================
    IEnumerator RollBoth(
        PageDie dieA,
        PageDie dieB,
        System.Action<int> onRollA,
        System.Action<int> onRollB)
    {
        bool doneA = false, doneB = false;
        if (diceUILeft != null)
            StartCoroutine(diceUILeft.Roll(
                dieA.data.minRoll, dieA.data.maxRoll,
                _ => doneA = true));
        else
        {
            dieA.roll = dieA.Roll();
            doneA = true;
        }
        if (diceUIRight != null)
            StartCoroutine(diceUIRight.Roll(
                dieB.data.minRoll, dieB.data.maxRoll,
                _ => doneB = true));
        else
        {
            dieB.roll = dieB.Roll();
            doneB = true;
        }
        yield return new WaitUntil(() => doneA && doneB);
        // Read lastRoll from UI as the canonical value
        int finalA = diceUILeft  != null ? diceUILeft.lastRoll  : dieA.roll;
        int finalB = diceUIRight != null ? diceUIRight.lastRoll : dieB.roll;
        // Sync back to PageDie so anything downstream reads the same number
        dieA.roll = finalA;
        dieB.roll = finalB;
        onRollA(finalA);
        onRollB(finalB);
    }
    // =========================
    // CAMERA HELPER — avoids repeating List construction everywhere
    // =========================
    IEnumerator CameraAction(
        CameraActionType type,
        Transform target         = null,
        List<Transform> targets  = null,
        float duration           = 0.3f,
        float shakeIntensity     = 0f)
    {
        if (combatCamera == null) yield break;
        yield return combatCamera.Play(new List<CameraAction>
        {
            new CameraAction
            {
                type           = type,
                target         = target,
                targets        = targets,
                duration       = duration,
                shakeIntensity = shakeIntensity
            }
        });
    }
}