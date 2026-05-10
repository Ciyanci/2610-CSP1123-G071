using UnityEngine;

public class InputTurnController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CombatFlowController.Instance
                .ConfirmPlanning();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CombatFlowController.Instance
                .AutoAssignPlayerActions();
        }
    }
}