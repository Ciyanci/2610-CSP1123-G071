using UnityEngine;

public class CombatInputController : MonoBehaviour
{
    void Update()
    {
        var flow = CombatFlowController.Instance;

        if (flow == null || !flow.inputEnabled)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryAssignTarget();
        }
    }

    void TryAssignTarget()
    {
        var flow = CombatFlowController.Instance;

        if (flow.selectedCard == null || flow.selectedUser == null)
            return;

        CharacterUnit target = GetUnitUnderMouse();

        if (target == null || target == flow.selectedUser)
            return;

        BattleFlowController.Instance.QueuePreview(
            flow.selectedUser,
            target,
            flow.selectedCard
        );

        var deck = flow.selectedUser.GetComponent<CharacterDeck>();
        deck.UseCard(flow.selectedCard);

        HandUI.Instance.Refresh(CombatFlowController.Instance.selectedUnit.deck);

        Debug.Log($"[INTENT] {flow.selectedUser.name} → {target.name}");

        flow.ClearSelection();
    }

    CharacterUnit GetUnitUnderMouse()
    {
        Vector2 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

        if (hit.collider != null)
            return hit.collider.GetComponent<CharacterUnit>();

        return null;
    }
}