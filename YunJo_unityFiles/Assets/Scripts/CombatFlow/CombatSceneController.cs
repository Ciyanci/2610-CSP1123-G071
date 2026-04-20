using UnityEngine;
using System.Collections.Generic;

public class CombatSceneController : MonoBehaviour
{
    public List<CharacterUnit> playerUnits;
    public List<CharacterUnit> enemyUnits;

    void Start()
    {
        SetupAllUnits();
    }

    void SetupAllUnits()
    {
        // Nothing needed anymore — CardDeck auto-finds owner
    }
}