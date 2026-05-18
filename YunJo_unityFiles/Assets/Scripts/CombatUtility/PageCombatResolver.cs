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

    //clash entry
    public IEnumerator ResolvePages(CombatPageRuntime a, CombatPageRuntime b)
    {
        CombatCardDisplayUI.Instance?.ShowForClash(a.owner, a.card, b.owner, b.card);
        diceGroupRight?.Bind(a.owner,  a.card);
        diceGroupLeft?.Bind(b.owner, b.card);
        yield return CameraAction(CameraActionType.FrameTargets,
            targets: new List<Transform> { a.owner.visual, b.owner.visual },
            duration: 0.4f);
        //both units should walk into lane simultaneously with this
        yield return LungeToLane(a.owner, b.owner);
        //clash loop
        while (!a.IsFinished && !b.IsFinished)
        {
            if (!a.owner.CanResolveAction() || !b.owner.CanResolveAction()) break;

            var dieA = a.GetCurrentDie();
            var dieB = b.GetCurrentDie();
            if (dieA == null || dieB == null) break;

            a.owner.PlayWindup();
            b.owner.PlayWindup();
            yield return new WaitForSeconds(windupDuration);

            int rollA = 0, rollB = 0;
            bool doneA = false, doneB = false;

            StartCoroutine(diceGroupLeft != null
                ? diceGroupLeft.RollCurrentDie(dieA.data.minRoll, dieA.data.maxRoll,
                    r => { rollA = r; doneA = true; })
                : DoneImmediate(dieA.Roll(), r => { rollA = r; doneA = true; }));

            StartCoroutine(diceGroupRight != null
                ? diceGroupRight.RollCurrentDie(dieB.data.minRoll, dieB.data.maxRoll,
                    r => { rollB = r; doneB = true; })
                : DoneImmediate(dieB.Roll(), r => { rollB = r; doneB = true; }));

            yield return new WaitUntil(() => doneA && doneB);

            dieA.roll = rollA;
            dieB.roll = rollB;

            yield return new WaitForSeconds(clashPauseDuration);

            if (rollA > rollB)
            {
                diceGroupLeft?.SetCurrentResult(true);
                diceGroupRight?.SetCurrentResult(false);
                yield return new WaitForSeconds(0.15f);

                diceGroupRight?.BreakCurrentDie();
                diceGroupLeft?.AdvanceDie();
                diceGroupRight?.AdvanceDie();

                yield return ApplyClashHit(a.owner, b.owner, dieA, rollA);
                b.Advance();
            }
            else if (rollB > rollA)
            {
                diceGroupRight?.SetCurrentResult(true);
                diceGroupLeft?.SetCurrentResult(false);
                yield return new WaitForSeconds(0.15f);

                diceGroupLeft?.BreakCurrentDie();
                diceGroupLeft?.AdvanceDie();
                diceGroupRight?.AdvanceDie();

                yield return ApplyClashHit(b.owner, a.owner, dieB, rollB);
                a.Advance();
            }
            else
            {
                diceGroupLeft?.SetCurrentResult(false);
                diceGroupRight?.SetCurrentResult(false);
                yield return new WaitForSeconds(0.15f);

                diceGroupLeft?.BreakCurrentDie();
                diceGroupRight?.BreakCurrentDie();
                diceGroupLeft?.AdvanceDie();
                diceGroupRight?.AdvanceDie();

                a.owner.sr.sprite = a.owner.idle;
                b.owner.sr.sprite = b.owner.idle;
                a.Advance();
                b.Advance();
            }

            yield return new WaitForSeconds(diePauseDuration);
        }

        yield return FlushRemaining(a, b.owner);
        yield return FlushRemaining(b, a.owner);

        a.owner.ResetPosition();
        b.owner.ResetPosition();

        diceGroupLeft?.Hide();
        diceGroupRight?.Hide();

        CombatCardDisplayUI.Instance?.Hide();
        yield return CameraAction(CameraActionType.Reset, duration: 0.3f);
    }
    IEnumerator LungeToLane(CharacterUnit unitA, CharacterUnit unitB)
    {
        if (ClashLane.Instance == null) yield break;

        //determine which side each unit is on by comparing X positions
        bool aIsLeft = unitA.transform.position.x < unitB.transform.position.x;

        Vector3 posA = aIsLeft
            ? ClashLane.Instance.LeftEngage
            : ClashLane.Instance.RightEngage;

        Vector3 posB = aIsLeft
            ? ClashLane.Instance.RightEngage
            : ClashLane.Instance.LeftEngage;

        //both go go
        Coroutine ca = StartCoroutine(unitA.MoveTo(posA, 0.3f));
        Coroutine cb = StartCoroutine(unitB.MoveTo(posB, 0.3f));
        yield return new WaitForSeconds(0.3f);
    }


    //flush toilet
    IEnumerator FlushRemaining(CombatPageRuntime page, CharacterUnit target)
    {
        while (!page.IsFinished)
        {
            if (!page.owner.CanResolveAction()) yield break;
            if (target == null || target.IsDead) yield break;

            var die = page.GetCurrentDie();
            if (die == null) { page.Advance(); continue; }

            yield return ApplyUnopposedHit(page.owner, target, die);

            diceGroupLeft?.AdvanceDie();
            page.Advance();

            yield return new WaitForSeconds(diePauseDuration);
        }
    }

    //single unopposed babababababa
    public IEnumerator ResolveSinglePage(CombatPageRuntime page)
    {
        if (page?.owner == null) yield break;

        CombatCardDisplayUI.Instance?.ShowForUnopposed(page.owner, page.card);
        diceGroupRight?.Bind(page.owner, page.card);

        yield return CameraAction(CameraActionType.FocusTarget,
            target: page.owner.visual, duration: 0.25f);

        while (!page.IsFinished)
        {
            if (!page.owner.CanResolveAction()) yield break;
            if (page.target == null || page.target.IsDead) yield break;

            var die = page.GetCurrentDie();
            if (die == null) { page.Advance(); continue; }

            yield return ApplyUnopposedHit(page.owner, page.target, die);

            diceGroupRight?.AdvanceDie();
            page.Advance();

            yield return new WaitForSeconds(diePauseDuration);
        }

        diceGroupRight?.Hide();
        CombatCardDisplayUI.Instance?.Hide();
        yield return CameraAction(CameraActionType.Reset, duration: 0.25f);
    }

    //clashy clashy
    IEnumerator ApplyClashHit(CharacterUnit attacker, CharacterUnit defender, PageDie die, int roll)
    {
        attacker.PlayAttack();
        defender.PlayHit();

        int damage = Mathf.Max(1, roll + die.Power);
        Vector3 attackDir = (defender.visual.position - attacker.visual.position).normalized;
        yield return defender.TakeDamageWithKnockback(damage, die.damageType, attackDir, false);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));

        yield return CameraAction(CameraActionType.Shake,
            shakeIntensity: clashShakeIntensity, duration: 0.2f);

        yield return new WaitForSeconds(hitPauseDuration);

        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;
    }

    //unopposed hitting
    IEnumerator ApplyUnopposedHit(CharacterUnit attacker, CharacterUnit defender, PageDie die)
    {
        attacker.PlayWindup();
        yield return new WaitForSeconds(windupDuration);

        //move to lane engage point based on which side attacker is on
        if (ClashLane.Instance != null)
        {
            bool attackerIsLeft = attacker.transform.position.x < defender.transform.position.x;
            Vector3 engagePos   = attackerIsLeft
                ? ClashLane.Instance.LeftEngage
                : ClashLane.Instance.RightEngage;

            yield return attacker.MoveTo(engagePos, 0.22f);
        }

        //roll while frozen in lane
        int roll = 0;
        if (diceGroupRight != null)
            yield return diceGroupRight.RollCurrentDie(
                die.data.minRoll, die.data.maxRoll, r => roll = r);
        else
            roll = die.Roll();

        diceGroupRight?.SetCurrentResult(true);

        attacker.PlayAttack();
        defender.PlayHit();

        int damage    = Mathf.Max(1, roll + die.Power);
        Vector3 dir   = (defender.visual.position - attacker.visual.position).normalized;
        yield return defender.TakeDamageWithKnockback(damage, die.damageType, dir, false);
        defender.TakeStaggerDamage(Mathf.Max(1, roll));

        yield return CameraAction(CameraActionType.Shake,
            shakeIntensity: hitShakeIntensity, duration: 0.15f);
        yield return new WaitForSeconds(hitPauseDuration);

        attacker.sr.sprite = attacker.idle;
        defender.sr.sprite = defender.idle;

        //walk back to start next action
        yield return attacker.MoveTo(attacker.GetStartPos(), 0.28f);
    }

    //helpers
    IEnumerator DoneImmediate(int val, System.Action<int> cb)
    {
        cb?.Invoke(val);
        yield break;
    }

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