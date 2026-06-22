using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string unitName;
    public Sprite portrait;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite hitSprite;
    public Sprite windupSprite;
    public Sprite moveSprite;


    [Header("Leader")]
    public bool        isLeader     = false;
    public KeypageData lockedKeypage;          // leaders only

    [Header("Base Stats")]
    public int baseMaxHP      = 100;
    public int baseMaxStagger = 50;
    public int maxLight       = 5;

    [Header("Base Resistances")]
    public DamageResistance baseResistances;

    [Header("Cards")]
    public List<CardData> starterDeck  = new();
    public List<CardData> uniqueCards  = new();  // leader only

    [Header("Passives")]
    public List<PassiveData> innatePassives = new();  // leader only

    // =========================
    // HELPERS
    // =========================
    public int GetMaxHP(KeypageData kp) =>
        baseMaxHP + (kp != null ? kp.hpBonus : 0);

    public int GetMaxStagger(KeypageData kp) =>
        baseMaxStagger + (kp != null ? kp.staggerBonus : 0);

    public DamageResistance GetResistances(KeypageData kp) =>
        (kp != null && kp.overrideResistances) ? kp.resistances : baseResistances;

    public List<CardData> GetFullCardPool(KeypageData kp)
    {
        var pool = new List<CardData>(starterDeck);
        foreach (var c in uniqueCards)
            if (!pool.Contains(c)) pool.Add(c);
        if (kp != null)
            foreach (var c in kp.grantedCards)
                if (!pool.Contains(c)) pool.Add(c);
        return pool;
    }

    public List<PassiveData> GetActivePassives(KeypageData kp)
    {
        var list = new List<PassiveData>(innatePassives);
        if (kp != null)
            foreach (var p in kp.grantedPassives)
                if (!list.Contains(p)) list.Add(p);
        return list;
    }
}
