using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public LineRenderer lr;
    public Transform tip;

    Transform start;
    Transform end;

    Card currentCard;
    CharacterUnit owner;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    public void Begin(CharacterUnit user, Card card)
    {
        owner = user;
        start = user.headAnchor;
        currentCard = card;

        gameObject.SetActive(true);

        Debug.Log($"[ARROW] Begin from {user.name} using {card.Data.Name}");
    }

    public void Set(Transform from, Transform to)
    {
        start = from;
        end = to;
    }

    public void SetPriority(int value)
    {
        lr.widthMultiplier = Mathf.Lerp(0.05f, 0.2f, value / 10f);
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

    CharacterUnit GetTargetUnit()
    {
        return end != null ? end.GetComponentInParent<CharacterUnit>() : null;
    }

    void Update()
    {
        if (start == null) return;

        Vector3 startPos = start.position + Vector3.up * 0.2f;

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
        // CLEAN CLASH PREVIEW (NO SCENE SCAN)
        // =========================
        CharacterUnit myTarget = GetTargetUnit();

        if (owner != null && myTarget != null)
        {
            foreach (var slot in owner.speedSlots)
            {
                if (slot.target == null || slot.assignedCard == null)
                    continue;

                if (slot.target == myTarget && slot.state == SlotState.Planned || slot.state == SlotState.Committed)
                {
                    finalEnd = (startPos + endPos) * 0.5f;
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
        // TIP
        // =========================
        if (tip != null)
        {
            tip.position = finalEnd;
            tip.right = (finalEnd - startPos).normalized;
        }
    }
}