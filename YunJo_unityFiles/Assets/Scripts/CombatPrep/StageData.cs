using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/StageData")]
public class StageData : ScriptableObject
{
    [Header("Identity")]
    public string stageName;

    [TextArea]
    public string description;

    public Sprite stageArt;

    [Header("Chapter")]
    public int chapterIndex;
    public bool isUnlocked = true;

    [Header("Scene")]
    //name of combat scene u wanna load
    public string combatSceneName = "Combat";

    [Header("Enemy Team")]
    //enemies spawned at battle start from unitdata
    public List<UnitData> enemyUnits = new();

    [Header("Enemy Prefab")]
    //the base prefab used for all spawned enemies
    //unitdata applied on top using ApplyUnitData()
    public GameObject enemyPrefab;
}
