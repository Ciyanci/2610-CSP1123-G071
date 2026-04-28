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

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);

        if (tip != null)
        {
            tip.position = endPos;
        }
    }
}