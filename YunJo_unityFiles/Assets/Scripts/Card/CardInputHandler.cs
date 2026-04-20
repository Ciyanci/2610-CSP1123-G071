using UnityEngine;

public class CardInputHandler : MonoBehaviour
{
    public void SetInputEnabled(bool enabled)
    {
        CombatFlowController.Instance.SetInputEnabled(enabled);
    }

    public void BeginTargeting(Card card, CharacterUnit user)
    {
        CombatFlowController.Instance.SelectCard(card, user);
    }

    public void ResetAll()
    {
        CombatFlowController.Instance.ResetAll();
    }
}