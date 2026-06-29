using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveData //using string to be saved since unity cant serialize scriptableobject references
{
    public List<string> unlockedCardNames = new();
}
