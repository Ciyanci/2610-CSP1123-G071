using UnityEngine;

public class ClashResult
{
    public CombatIntent a;
    public CombatIntent b;

    public CombatRoll aRoll;
    public CombatRoll bRoll;

    public CombatIntent winner;
    public CombatIntent loser;

    public bool IsDraw => winner == null && loser == null;
}