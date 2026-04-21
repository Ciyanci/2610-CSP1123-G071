using UnityEngine;

public class CombatSceneController : MonoBehaviour
{
    void Start()
    {
        UnitRegistry.Instance.Refresh();
    }
}