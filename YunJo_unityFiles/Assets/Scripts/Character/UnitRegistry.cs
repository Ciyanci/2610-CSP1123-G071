using System.Collections.Generic;
using UnityEngine;

public class UnitRegistry : MonoBehaviour
{
    public static UnitRegistry Instance;

    public List<CharacterUnit> players = new();
    public List<CharacterUnit> enemies = new();

    void Awake()
    {
        Instance = this;
        Refresh();
    }

    public void Refresh()
    {
        players.Clear();
        enemies.Clear();

        var all = FindObjectsByType<CharacterUnit>(FindObjectsInactive.Exclude);

        foreach (var u in all)
        {
            if (u.CompareTag("Player"))
                players.Add(u);
            else if (u.CompareTag("Enemy"))
                enemies.Add(u);
        }
    }
}