using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatLaneManager : MonoBehaviour
{
    public static CombatLaneManager Instance;

    void Awake() => Instance = this;

    public IEnumerator MoveActorsToLane(List<CombatIntent> activeIntents)
    {
        if (activeIntents == null || activeIntents.Count == 0) yield break;
        if (ClashLane.Instance == null)
        {
            Debug.LogError("[LANE] ClashLane.Instance is null");
            yield break;
        }

        float moveDuration = 0.28f;

        foreach (var intent in activeIntents)
        {
            if (intent?.user == null || intent.user.IsDead) continue;

            bool isLeft  = IsLeftSide(intent.user);
            int  index   = GetSideIndex(intent.user, isLeft);

            //offset the engage point based on clashAnchor world position
            Vector3 laneEngage = ClashLane.Instance.GetEngagePosition(isLeft, index);
            Vector3 anchor     = intent.user.clashAnchor != null
                ? intent.user.clashAnchor.position
                : intent.user.transform.position;

            //blend between anchor position and lane engage — keeps unit grounded
            Vector3 dest = new Vector3(laneEngage.x, anchor.y, laneEngage.z);

            Debug.Log($"[LANE] {intent.user.unitName} → {dest}");
            StartCoroutine(intent.user.MoveTo(dest, moveDuration));
        }

        yield return new WaitForSeconds(moveDuration);
    }


    public bool IsLeftSide(CharacterUnit unit)
    {
        if (ClashLane.Instance == null) return true;
        return unit.transform.position.x < ClashLane.Instance.Centre.x;
    }

    int GetSideIndex(CharacterUnit unit, bool isLeft)
    {
        var players = UnitRegistry.Instance.players;
        var enemies = UnitRegistry.Instance.enemies;

        if (isLeft)
        {
            int pi = players.IndexOf(unit);
            if (pi >= 0) return pi;
            int ei = enemies.IndexOf(unit);
            if (ei >= 0) return ei;
        }
        else
        {
            int ei = enemies.IndexOf(unit);
            if (ei >= 0) return ei;
            int pi = players.IndexOf(unit);
            if (pi >= 0) return pi;
        }

        return 0;
    }

    public IEnumerator ReturnAllToStart(HashSet<CharacterUnit> exclude = null)
    {
        if (UnitRegistry.Instance == null) yield break;

        var allUnits = new List<CharacterUnit>();
        allUnits.AddRange(UnitRegistry.Instance.players);
        allUnits.AddRange(UnitRegistry.Instance.enemies);

        float duration = 0.28f;

        foreach (var unit in allUnits)
        {
            if (unit == null || unit.IsDead) continue;
            if (exclude != null && exclude.Contains(unit)) continue;
            StartCoroutine(unit.MoveTo(unit.GetStartPos(), duration));
        }

        yield return new WaitForSeconds(duration);
    }
}
