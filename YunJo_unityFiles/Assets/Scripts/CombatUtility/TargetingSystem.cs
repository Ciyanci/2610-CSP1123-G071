using UnityEngine;
using System.Collections.Generic;

public class TargetingSystem : MonoBehaviour
{
    private List<(CharacterUnit a, CharacterUnit b)> intents = new();

    public void RegisterIntent(CharacterUnit attacker, CharacterUnit target)
    {
        intents.Add((attacker, target));
    }

    public void ClearIntents()
    {
        intents.Clear();
    }

    public bool IsClashing(CharacterUnit a, CharacterUnit b)
    {
        bool ab = false;
        bool ba = false;

        foreach (var i in intents)
        {
            if (i.a == a && i.b == b) ab = true;
            if (i.a == b && i.b == a) ba = true;
        }

        return ab && ba;
    }
}