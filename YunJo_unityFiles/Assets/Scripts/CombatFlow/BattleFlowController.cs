using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public static BattleFlowController Instance;

    public CombatCamera cam;
    public ClashSystem clashSystem;

    [Header("Arrow")]
    public ArrowController arrowPrefab;

    // =========================
    // PREVIEW LAYER
    // =========================
    public List<PreviewIntent> previewIntents = new();

    // =========================
    // COMBAT LAYER
    // =========================
    public List<CombatIntent> intents = new();
    List<(CombatIntent, CombatIntent)> clashes = new();

    void Awake()
    {
        Instance = this;
    }

    // =====================================================
    // PREVIEW
    // =====================================================
    public void QueuePreview(CharacterUnit user, CharacterUnit target, Card card)
    {
        if (user == null || target == null || card == null)
            return;

        ArrowController arrow = Instantiate(arrowPrefab, transform);
        arrow.Set(user.headAnchor, target.headAnchor);

        PreviewIntent preview = new PreviewIntent
        {
            user = user,
            target = target,
            card = card,
            arrow = arrow
        };

        previewIntents.Add(preview);
    }

    // =====================================================
    // COMBAT BUILD
    // =====================================================
    public void BuildCombatIntents()
    {
        intents.Clear();

        foreach (var p in previewIntents)
        {
            if (p == null || p.user == null || p.target == null) continue;

            intents.Add(new CombatIntent
            {
                user = p.user,
                target = p.target,
                card = p.card,
                resolved = false
            });
        }
    }

    // =====================================================
    // RESOLVE ALL
    // =====================================================
    public IEnumerator ResolveAll()
    {
        BuildCombatIntents();
        BuildClashes();

        ClearPreview(); // 🔥 IMPORTANT (fixes arrow bug)

        yield return cam.Reset();

        foreach (var c in clashes)
        {
            yield return clashSystem.Resolve(c.Item1, c.Item2);
            c.Item1.resolved = true;
            c.Item2.resolved = true;
        }

        foreach (var i in intents)
        {
            if (i == null || i.resolved)
                continue;

            yield return ResolveSingle(i);
        }

        Cleanup();

        if (CombatFlowController.Instance != null)
            CombatFlowController.Instance.SetInputEnabled(false);
    }

    // =====================================================
    // CLASH BUILD
    // =====================================================
    void BuildClashes()
    {
        clashes.Clear();

        HashSet<CombatIntent> used = new();

        for (int i = 0; i < intents.Count; i++)
        {
            var a = intents[i];
            if (a == null || used.Contains(a)) continue;

            for (int j = i + 1; j < intents.Count; j++)
            {
                var b = intents[j];
                if (b == null || used.Contains(b)) continue;

                bool clash =
                    a.user == b.target &&
                    a.target == b.user;

                if (clash)
                {
                    clashes.Add((a, b));
                    used.Add(a);
                    used.Add(b);
                    break;
                }
            }
        }
    }

    // =====================================================
    // UNOPPOSED
    // =====================================================
    IEnumerator ResolveSingle(CombatIntent i)
    {
        if (i.user == null || i.target == null)
            yield break;

        i.user.SetCombatStartPosition();
        i.target.SetCombatStartPosition();

        yield return i.user.MoveTo(i.target.clashAnchor.position);

        i.user.PlayAttack();
        i.target.PlayHit();

        int dmg = Random.Range(i.card.min, i.card.max + 1);
        i.target.TakeDamage(dmg);

        yield return new WaitForSeconds(0.3f);

        i.user.ResetPosition();
        i.target.ResetPosition();
    }

    // =====================================================
    // CLEANUP COMBAT
    // =====================================================
    void Cleanup()
    {
        intents.Clear();
        clashes.Clear();
    }

    // =====================================================
    // PREVIEW CLEANUP (FIXED - THIS WAS MISSING)
    // =====================================================
    public void ClearPreview()
    {
        foreach (var p in previewIntents)
        {
            if (p?.arrow != null)
                Destroy(p.arrow.gameObject);
        }

        previewIntents.Clear();
    }

    // Optional alias (fixes older scripts calling this)
    public void HidePreviewArrows()
    {
        ClearPreview();
    }
}
//kill me