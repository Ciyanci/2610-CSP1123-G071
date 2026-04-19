using UnityEngine;

public class CardInputHandler : MonoBehaviour
{
    public LineRenderer arrow;
    private Card currentCard;
    private CharacterUnit owner;

    bool dragging;

    public void BeginDrag(Card card, CharacterUnit unit)
    {
        currentCard = card;
        owner = unit;
        dragging = true;

        arrow.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!dragging) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        arrow.SetPosition(0, owner.headAnchor.position);
        arrow.SetPosition(1, mouse);

        if (Input.GetMouseButtonUp(0))
        {
            TryTarget(mouse);
            dragging = false;
            arrow.gameObject.SetActive(false);
        }
    }

    void TryTarget(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pos);
        if (hit == null) return;

        CharacterUnit target = hit.GetComponent<CharacterUnit>();
        if (target == null) return;

        FindFirstObjectByType<BattleFlowController>()
            .QueueAction(owner, target, currentCard);
    }
}