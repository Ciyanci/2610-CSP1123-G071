using UnityEngine;
using System.Collections.Generic;

public class TargetingSystem : MonoBehaviour
{
    public List<CharacterUnit> enemies;

    public CharacterUnit selectedTarget;

    public void SelectTarget(CharacterUnit t)
    {
        selectedTarget = t;
    }
}