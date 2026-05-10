using UnityEngine;

[System.Serializable]
public class DiceData
{
    public int minRoll = 1;
    public int maxRoll = 6;

    public int power = 0;

    public DamageType damageType;

    public DiceBehaviour effect;
}