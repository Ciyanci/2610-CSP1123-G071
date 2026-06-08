using UnityEngine;
public class ClashLane : MonoBehaviour
{
    public static ClashLane Instance;
    [Header("Lane Markers")]
    public Transform centrePoint;
    public Transform leftEngagePoint;
    public Transform rightEngagePoint;
    public Transform cameraFocusPoint;
    [Header("Clash Stand Offset — distance each unit stands from meet point")]
    public float clashStandOffset = 5f;
    [Header("Attack Near Offset — attacker stops this far from defender")]
    public float attackNearOffset = 5f;
    [Header("Side Stagger — for multiple units on same side")]
    public float sideStaggerX = 0.6f;
    public float sideStaggerY = 0f;
    void Awake() => Instance = this;
    public Vector3 Centre      => centrePoint.position;
    public Vector3 CameraFocus => cameraFocusPoint != null
        ? cameraFocusPoint.position
        : centrePoint.position;
    public Vector3 GetClashStandPosition(CharacterUnit unit, CharacterUnit opponent)
    {
        if (opponent.clashAnchor != null)
            return opponent.clashAnchor.position;
        bool unitIsLeft = unit.transform.position.x < opponent.transform.position.x;
        float sign      = unitIsLeft ? -1f : 1f;
        return Centre + new Vector3(sign * clashStandOffset, 0f, 0f);
    }
    public Vector3 GetAttackPositionNear(CharacterUnit attacker, CharacterUnit defender)
    {
        if (defender.clashAnchor != null)
            return defender.clashAnchor.position;
        bool attackerIsLeft = attacker.transform.position.x < defender.visual.position.x;
        float sign          = attackerIsLeft ? -1f : 1f;
        return defender.visual.position + new Vector3(sign * attackNearOffset, 0f, 0f);
    }
    public Vector3 GetEngagePosition(bool isLeft, int slotIndex = 0)
    {
        Transform anchor = isLeft ? leftEngagePoint : rightEngagePoint;
        float xSign      = isLeft ? -1f : 1f;
        return anchor.position
            + new Vector3(xSign * slotIndex * sideStaggerX,
                          slotIndex * sideStaggerY, 0f);
    }
}