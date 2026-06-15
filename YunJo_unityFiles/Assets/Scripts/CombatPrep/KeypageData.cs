using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Keypage")]
public class KeypageData : ScriptableObject
{
    [Header("Identity")]
    public string keypageName;
    public Sprite art;

    [Header("Stat Bonuses — added on top of UnitData base")]
    public int hpBonus      = 0;
    public int staggerBonus = 0;

    [Header("Resistances — overrides unit base if assigned")]
    public bool overrideResistances = false;
    public DamageResistance resistances;

    [Header("Granted Content")]
    public List<PassiveData> grantedPassives = new();
    public List<CardData>    grantedCards    = new();
}
