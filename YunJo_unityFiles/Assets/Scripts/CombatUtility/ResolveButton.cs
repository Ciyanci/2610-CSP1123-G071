using UnityEngine;

public class ResolveButton : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("[BUTTON] Resolve combat");

        CombatFlowController.Instance.SetInputEnabled(false);

        StartCoroutine(
            BattleFlowController.Instance.ResolveAll()
        );
    }
}