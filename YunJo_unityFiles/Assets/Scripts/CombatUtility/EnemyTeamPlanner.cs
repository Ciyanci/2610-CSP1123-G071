using UnityEngine;
using System.Collections.Generic;
public class EnemyTeamPlanner : MonoBehaviour
{
    public List<EnemyAI> enemies;
    public BattleFlowController flow;

    public void Plan()
    {
        foreach (var enemy in enemies)
        {
            StartCoroutine(enemy.TakeTurn());
        }
    }
}