using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public CombatCamera cam;
    public ClashSystem clashSystem;

    public ArrowController arrowPrefab;

    public List<PreviewIntent> previewIntents = new();
    public List<CombatIntent> intents = new();

    List<(CombatIntent, CombatIntent)> clashes = new();

    // ======================
    // PREVIEW
    // ======================

    public void QueuePreview(CharacterUnit user, CharacterUnit target, Card card)
    {
        ArrowController arrow = Instantiate(arrowPrefab, transform);
        arrow.Set(user.headAnchor, target.headAnchor);
        Debug.Log($"[PREVIEW] {user.name} → {target.name} using {card.Data.Name}");

        previewIntents.Add(new PreviewIntent
        {
            user = user,
            target = target,
            card = card,
            arrow = arrow
        });
    }

    // ======================
    // RESOLVE
    // ======================

    public IEnumerator ResolveAll()
    {
        ConvertPreviewToCombat();
        BuildClashes();
        HidePreview();

        yield return cam.Reset();

        foreach (var c in clashes)
        {
            yield return clashSystem.Resolve(c.Item1, c.Item2);
            c.Item1.resolved = true;
            c.Item2.resolved = true;
        }

        foreach (var i in intents)
        {
            if (!i.resolved)
                yield return ResolveSingle(i);
        }

        Cleanup();

        CombatFlowController.Instance.SetInputEnabled(false);
    }

    // ======================
    // BUILD
    // ======================

    void ConvertPreviewToCombat()
    {
        intents.Clear();

        foreach (var p in previewIntents)
        {
            intents.Add(new CombatIntent
            {
                user = p.user,
                target = p.target,
                card = p.card,
                resolved = false
            });
        }
    }

    void BuildClashes()
    {
        clashes.Clear();
        HashSet<CombatIntent> used = new();

        for (int i = 0; i < intents.Count; i++)
        {
            var a = intents[i];
            if (used.Contains(a)) continue;

            for (int j = i + 1; j < intents.Count; j++)
            {
                var b = intents[j];
                if (used.Contains(b)) continue;

                if (a.user == b.target && a.target == b.user)
                {
                    clashes.Add((a, b));
                    used.Add(a);
                    used.Add(b);
                    break;
                }
            Debug.Log($"[COMBAT] Building {previewIntents.Count} intents");
            }
        }
        Debug.Log($"[COMBAT] Clashes found: {clashes.Count}");
    }

    // ======================
    // UNOPPOSED
    // ======================

    IEnumerator ResolveSingle(CombatIntent i)
    {
        if (i.user == null || i.target == null)
            yield break;

        var cam = this.cam;

        i.user.SetCombatStartPosition();
        i.target.SetCombatStartPosition();

        Vector3 attackPos = i.target.clashAnchor.position;

        // -------------------------
        // MOVE INTO RANGE
        // -------------------------
        yield return i.user.MoveTo(attackPos);

        // -------------------------
        // 🎥 ZOOM IN ON ATTACK
        // -------------------------
        if (cam != null)
            yield return cam.Focus(i.user.visual.position, 0.4f, 0.3f);

        // -------------------------
        // 💥 WINDUP ANIMATION
        // -------------------------
        i.user.PlayWindup();   // 🔥 you need to add this
        yield return new WaitForSeconds(0.4f);

        // -------------------------
        // 🎲 ROLL DAMAGE DICE
        // -------------------------
        int roll = Random.Range(i.card.Min, i.card.Max + 1);

        Debug.Log($"[UNOPPOSED] {i.user.name} rolled {roll}");

        yield return new WaitForSeconds(0.2f);

        // -------------------------
        // 💥 ATTACK
        // -------------------------
        i.user.PlayAttack();
        i.target.PlayHit();

        i.target.TakeDamage(roll);

        yield return new WaitForSeconds(0.25f);

        // -------------------------
        // 🎥 RESET CAMERA
        // -------------------------
        if (cam != null)
            yield return cam.Reset();

        // -------------------------
        // RESET POSITIONS
        // -------------------------
        i.user.ResetPosition();
        i.target.ResetPosition();
    }

    // ======================
    // CLEANUP
    // ======================

    void Cleanup()
    {
        intents.Clear();
        clashes.Clear();

        foreach (var p in previewIntents)
        {
            if (p.arrow != null)
                Destroy(p.arrow.gameObject);
        }

        previewIntents.Clear();
    }

    void HidePreview()
    {
        foreach (var p in previewIntents)
        {
            if (p.arrow != null)
                p.arrow.gameObject.SetActive(false);
        }
    }

    public void ClearPreview()
    {
        foreach (var p in previewIntents)
        {
            if (p.arrow != null)
                Destroy(p.arrow.gameObject);
        }

        previewIntents.Clear();
    }
}