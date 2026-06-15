using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string unitName;
    public Sprite portrait;
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite hitSprite;
    public Sprite windupSprite;
    public Sprite moveSprite;

    [Header("Leader")]
    public bool isLeader = false;

    //leaders have a fixed keypage that cannot be changed
    public KeypageData lockedKeypage;

    [Header("Base Stats")]
    public int baseMaxHP      = 100;
    public int baseMaxStagger = 50;
    public int maxLight       = 5;

    [Header("Base Resistances")]
    public DamageResistance baseResistances;

    [Header("Deck")]
    //cards always available to this unit regardless of keypage
    public List<CardData> starterDeck = new();

    //unique cards available to certain characters if needed
    public List<CardData> uniqueCards = new();

    [Header("Passives")]
    //leader-only, always active regardless of keypage
    public List<PassiveData> innatePassives = new();

    //return full card pool to character (starter + unique + keypage granted cards)
    public List<CardData> GetFullCardPool(KeypageData keypage)
    {
        var pool = new List<CardData>(starterDeck);

        foreach (var c in uniqueCards)
            if (!pool.Contains(c)) pool.Add(c);

        if (keypage != null)
            foreach (var c in keypage.grantedCards)
                if (!pool.Contains(c)) pool.Add(c);

        return pool;
    }

    //return active passive for unit
    public List<PassiveData> GetActivePassives(KeypageData keypage)
    {
        var passives = new List<PassiveData>(innatePassives);

        if (keypage != null)
            foreach (var p in keypage.grantedPassives)
                if (!passives.Contains(p)) passives.Add(p);

        return passives;
    }

    //final hp after keypage bonus (negative value to reduce max hp)
    public int GetMaxHP(KeypageData keypage)
    {
        return baseMaxHP + (keypage != null ? keypage.hpBonus : 0);
    }

    //final stagger threshold
    public int GetMaxStagger(KeypageData keypage)
    {
        return baseMaxStagger + (keypage != null ? keypage.staggerBonus : 0);
    }

    //keypage overrides resistances (usually wont be touched but added just in caaase)
    public DamageResistance GetResistances(KeypageData keypage)
    {
        if (keypage != null && keypage.overrideResistances)
            return keypage.resistances;
        return baseResistances;
    }
}
