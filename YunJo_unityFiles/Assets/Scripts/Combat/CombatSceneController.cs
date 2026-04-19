using UnityEngine;
using System.Collections.Generic;

public class CombatSceneController : MonoBehaviour
{
    public List<CharacterUnit> playerUnits;
    public List<CharacterUnit> enemyUnits;

    public void Setup(List<CharacterUnit> selectedEnemies)
    {
        enemyUnits = selectedEnemies;

        // spawn + position them
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].transform.position = new Vector3(3 + i, 0, 0);
        }
    }
}