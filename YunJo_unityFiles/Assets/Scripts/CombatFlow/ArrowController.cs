using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public LineRenderer lr;
    public Transform tip;

    Transform start;
    Transform end;

    // 🔥 FIX: store card + user for future logic (preview, clash, UI)
    Card currentCard;
    CharacterUnit owner;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    // =========================
    // BEGIN (FROM CARD SELECT)
    // =========================
    public void Begin(CharacterUnit user, Card card)
    {
        owner = user;
        start = user.headAnchor;
        currentCard = card;

        gameObject.SetActive(true);

        Debug.Log($"[ARROW] Begin from {user.name} using {card.Data.Name}");
    }

    // =========================
    // SET TARGET (OPTIONAL USE)
    // =========================
    public void Set(Transform from, Transform to)
    {
        start = from;
        end = to;
    }

    public void SetTarget(CharacterUnit target)
    {
        if (target == null) return;

        end = target.headAnchor;

        Debug.Log($"[ARROW] Target set: {target.name}");
    }

    public void End()
    {
        start = null;
        end = null;
        currentCard = null;
        owner = null;

        gameObject.SetActive(false);
    }

    // =========================
    // HELPERS
    // =========================
    CharacterUnit GetStartUnit()
    {
        return owner;
    }

    CharacterUnit GetTargetUnit()
    {
        return end != null ? end.GetComponentInParent<CharacterUnit>() : null;
    }

    // =========================
    // UPDATE (DRAW + CLASH PREVIEW)
    // =========================
    void Update()
    {
        if (start == null) return;

        Vector3 startPos = start.position + Vector3.up * 0.2f;

        // 🔥 FOLLOW MOUSE IF NO TARGET YET
        Vector3 endPos;

        if (end == null)
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            endPos = mouse;
        }
        else
        {
            endPos = end.position + Vector3.up * 0.2f;
        }

        Vector3 finalEnd = endPos;

        // =========================
        // CLASH DETECTION (PREVIEW)
        // =========================
        var flow = FindFirstObjectByType<BattleFlowController>();

        if (flow != null && owner != null)
        {
            foreach (var p in flow.previewIntents)
            {
                if (p.user == null || p.target == null) continue;

                bool clash =
                    p.user == GetTargetUnit() &&
                    p.target == owner;

                if (clash)
                {
                    // 🔥 midpoint collision (LoR style)
                    finalEnd = (startPos + endPos) * 0.5f;

                    Debug.Log("[ARROW] Clash preview detected");
                    break;
                }
            }
        }

        // =========================
        // DRAW LINE
        // =========================
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, finalEnd);

        // =========================
        // TIP ROTATION
        // =========================
        if (tip != null)
        {
            tip.position = finalEnd;
            tip.right = (finalEnd - startPos).normalized;
        }
    }
}