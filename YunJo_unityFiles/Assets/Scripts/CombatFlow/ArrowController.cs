using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public LineRenderer lr;
    public Transform tip;

    public void Begin(CharacterUnit user, Card card)
    {
        start = user.headAnchor;
        gameObject.SetActive(true);
    }

    Transform start;
    Transform end;

    Camera cam;

    CharacterUnit GetStartUnit()
    {
        return start != null ? start.GetComponentInParent<CharacterUnit>() : null;
    }

    CharacterUnit GetTargetUnit()
    {
        return end != null ? end.GetComponentInParent<CharacterUnit>() : null;
    }

    void Awake()
    {
        cam = Camera.main;
    }

    public void Set(Transform from, Transform to)
    {
        start = from;
        end = to;
    }

    public void End()
    {
        start = null;
        end = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (start == null || end == null) return;

        Vector3 startPos = start.position + Vector3.up * 0.2f;
        Vector3 endPos = end.position + Vector3.up * 0.2f;

        // 🔥 CHECK FOR CLASH MIDPOINT
        Vector3 finalEnd = endPos;

        var flow = FindFirstObjectByType<BattleFlowController>();

        if (flow != null)
        {
            foreach (var p in flow.previewIntents)
            {
                if (p.user == null || p.target == null) continue;

                // find reverse intent
                bool clash =
                    p.user == GetTargetUnit() &&
                    p.target == GetStartUnit();

                if (clash)
                {
                    finalEnd = (startPos + endPos) * 0.5f;
                    break;
                }
            }
        }

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, finalEnd);

        if (tip != null)
        {
            tip.position = finalEnd;
            tip.right = (finalEnd - startPos).normalized;
        }
    }
}