using UnityEngine;
using System.Collections.Generic;

public class CombatHUDController : MonoBehaviour
{
    public static CombatHUDController Instance;

    [Header("Player Entries — bottom left, top to bottom")]
    public List<UnitHUDEntry> playerEntries = new();

    [Header("Enemy Entries — bottom right, top to bottom")]
    public List<UnitHUDEntry> enemyEntries = new();

    void Awake() => Instance = this;

    //bind (i want to be binded by manhattan cafe - shu)
    public void Bind()
    {
        var players = UnitRegistry.Instance.players;
        var enemies = UnitRegistry.Instance.enemies;

        for (int i = 0; i < playerEntries.Count; i++)
        {
            if (i < players.Count && players[i] != null)
                playerEntries[i].Bind(players[i]);
            else
                playerEntries[i].Unbind();
        }

        for (int i = 0; i < enemyEntries.Count; i++)
        {
            if (i < enemies.Count && enemies[i] != null)
                enemyEntries[i].Bind(enemies[i]);
            else
                enemyEntries[i].Unbind();
        }
    }

    //refresh all chocolate bars (info bar on bottom u know what this is bro)
    public void RefreshAll()
    {
        foreach (var e in playerEntries) e.Refresh();
        foreach (var e in enemyEntries)  e.Refresh();
    }

    //show speed slot bubble booblbleb
    public void ShowSpeedBubbles()
    {
        var players = UnitRegistry.Instance.players;
        var enemies = UnitRegistry.Instance.enemies;

        for (int i = 0; i < playerEntries.Count; i++)
        {
            if (i >= players.Count || players[i] == null) continue;
            var unit = players[i];
            if (unit.speedSlots.Count > 0)
                playerEntries[i].ShowSpeedBubble(unit.speedSlots[0].value);
        }

        for (int i = 0; i < enemyEntries.Count; i++)
        {
            if (i >= enemies.Count || enemies[i] == null) continue;
            var unit = enemies[i];
            if (unit.speedSlots.Count > 0)
                enemyEntries[i].ShowSpeedBubble(unit.speedSlots[0].value);
        }
    }
}
