using UnityEngine;

[System.Serializable]
public class DefensiveDie
{
    public DefenseType type;

    public int value; // strength of defense roll

    public int Roll()
    {
        value = Random.Range(1, 8);
        return value;
    }
}