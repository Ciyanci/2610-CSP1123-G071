using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public static BattleFlowController Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
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
        if (user == null || target == null || card == null) return;

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

        // 🎥 RESET CAMERA
        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction { type = CameraActionType.Reset }
        });

        // -------------------------
        // CLASHES
        // -------------------------
        foreach (var c in clashes)
        {
            yield return clashSystem.Resolve(c.Item1, c.Item2);
            c.Item1.resolved = true;
            c.Item2.resolved = true;
        }

        // -------------------------
        // UNOPPOSED
        // -------------------------
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

        Debug.Log($"[COMBAT] Built {intents.Count} intents");
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

                    Debug.Log($"[CLASH FOUND] {a.user.name} <-> {b.user.name}");
                    break;
                }
            }
        }

        Debug.Log($"[COMBAT] Total clashes: {clashes.Count}");
    }

    //unopp atk

    IEnumerator ResolveSingle(CombatIntent i)
    {
        if (i.user == null || i.target == null)
            yield break;

        i.user.SetCombatStartPosition();
        i.target.SetCombatStartPosition();

        //pos
        Vector3 dir = (i.target.visual.position - i.user.visual.position).normalized;

        float offset = 5f;

        Vector3 attackPos = i.target.clashAnchor.position - dir * offset;

        yield return i.user.MoveTo(attackPos);

        //camfoc
        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction
            {
                type = CameraActionType.FocusTarget,
                target = i.user.visual,
                duration = 0.25f
            },
            new CameraAction
            {
                type = CameraActionType.Zoom,
                zoom = 3.2f,
                duration = 0.25f
            }
        });

        // -------------------------
        // 🎲 WINDUP + ROLL (SYNCED)
        // -------------------------
        i.user.PlayWindup();

        int roll = 0;
        float rollTime = 0.45f;

        float t = 0;

        while (t < rollTime)
        {
            roll = Random.Range(i.card.Min, i.card.Max + 1);
            t += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"[UNOPPOSED] {i.user.name} rolled {roll}");

        // -------------------------
        // 🎯 CINEMATIC PAUSE
        // -------------------------
        yield return new WaitForSeconds(0.15f);

        // -------------------------
        // 💥 DAMAGE CALCULATION
        // -------------------------
        float normalized = Mathf.InverseLerp(i.card.Min, i.card.Max, roll);

        int finalDamage = Mathf.RoundToInt(
            Mathf.Lerp(i.card.Min, i.card.Max, normalized)
        );

        // optional scaling boost
        finalDamage = Mathf.Max(1, finalDamage);

        // -------------------------
        // 💥 ATTACK
        // -------------------------
        i.user.PlayAttack();
        i.target.PlayHit();

        i.target.TakeDamage(finalDamage);

        Debug.Log($"[UNOPPOSED] Damage dealt: {finalDamage}");

        yield return new WaitForSeconds(0.25f);

        // -------------------------
        // 🎥 CAMERA RESET
        // -------------------------
        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction { type = CameraActionType.Reset }
        });

        // -------------------------
        // 🔄 RESET POSITIONS
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