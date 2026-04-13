using UnityEngine;

public class DiceSystem
{
    public static int Roll(int min, int max)
    {
        return Random.Range(min, max+1);
    }
}
