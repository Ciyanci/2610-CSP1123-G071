using UnityEngine;
using System.Collections.Generic;

public class SpeedDiceSystem : MonoBehaviour
{
    public Dictionary<CharacterUnit, int> speedRolls = new();

    public void RollAll(List<CharacterUnit> units)
    {
        speedRolls.Clear();

        foreach (var u in units)
        {
            speedRolls[u] = Random.Range(1, 6);
        }
    }
}