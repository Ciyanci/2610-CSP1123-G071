using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public LineRenderer lr;
    public Transform tip;

    RectTransform startUI;
    RectTransform endUI;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        lr.positionCount = 2;
    }

    public void Begin(RectTransform cardUI)
    {
        startUI = cardUI;
        endUI = null;

        gameObject.SetActive(true);
    }

    public void SetTarget(SpeedSlot slot)
    {
        if (slot?.ui == null) return;

        endUI = slot.ui.GetComponent<RectTransform>();
    }

    public void End()
    {
        startUI = null;
        endUI = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (startUI == null) return;

        Vector3 start = startUI.position;
        Vector3 end = endUI != null
            ? endUI.position
            : cam.ScreenToWorldPoint(Input.mousePosition);

        end.z = 0;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        if (tip != null)
        {
            tip.position = end;
            tip.right = (end - start).normalized;
        }
    }
}