using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public LineRenderer lr;
    public Transform tip;

    Transform start;
    Card currentCard;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    public void Begin(CharacterUnit user, Card card)
    {
        start = user.headAnchor;
        currentCard = card;
        gameObject.SetActive(true);
    }

    public void ForceRefresh(CharacterUnit user, Card card)
    {
        Begin(user, card);
    }

    public void End()
    {
        start = null;
        currentCard = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (start == null) return;

        Vector3 startPos = start.position + Vector3.up * 0.2f;

        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, mouse);

        if (tip != null)
        {
            tip.position = mouse;
            tip.right = (mouse - startPos).normalized;
        }

        //left click to confirm target
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouse);

            if (hit == null) return;

            CharacterUnit target = hit.GetComponent<CharacterUnit>();

            if (target != null)
            {
                CombatFlowController.Instance.ConfirmTarget(target);
            }
        }

        //right click to cancel
        if (Input.GetMouseButtonDown(1))
        {
            CombatFlowController.Instance.ResetAll();
        }
    }
}