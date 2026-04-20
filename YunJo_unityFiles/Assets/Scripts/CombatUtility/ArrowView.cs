using UnityEngine;

public class ArrowView : MonoBehaviour
{
    public Transform start;
    public Transform tip;
    public Transform target;

    private LineRenderer lr;

    private Vector3 oldStart;
    private Vector3 oldTarget;
    private Vector3 tempTarget;

    public void SetStart(Transform t)
    {
    start = t;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void SetTip(Transform t)
    {
        tip = t;
    }

    public void SetTargetPoint(Vector3 worldPos)
    {
        target = null;
        tempTarget = worldPos;
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!start || lr == null) return;

        Vector3 startPos = start.position;

        Vector3 targetPos = target != null ? target.position : tempTarget;

        oldStart = Vector3.Lerp(oldStart, startPos, 0.2f);
        oldTarget = Vector3.Lerp(oldTarget, targetPos, 0.2f);

        startPos = oldStart;
        targetPos = oldTarget;

        startPos.y += 0.2f;
        targetPos.y += 0.2f;

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, targetPos);

        if (tip != null)
        {
            Vector3 dir = (targetPos - startPos).normalized;

            tip.position = targetPos;
            tip.right = dir;
        }
    }
}