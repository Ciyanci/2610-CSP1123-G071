using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterUnit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName;

    public UnitType unitType;

    [Header("Stats")]
    public int maxHP = 100;
    public int hp = 100;

    public int maxStagger = 50;
    public int stagger = 50;

    [Header("Resources")]
    public int maxLight = 3;
    public int currentLight = 3;

    [Header("Resistances")]
    public DamageResistance resistances;

    [Header("Deck")]
    public CharacterDeck deck;

    [Header("Speed Dice")]
    public List<SpeedDie> speedDice = new();

    [Header("Visual")]
    public Transform visual;
    public Transform headAnchor;
    public Transform clashAnchor;

    [Header("Sprites")]
    public SpriteRenderer sr;

    public Sprite idle;
    public Sprite attack;
    public Sprite hit;
    public Sprite windup;

    Vector3 startPos;

    void Awake()
    {
        startPos = visual.position;

        if (deck == null)
            deck = GetComponent<CharacterDeck>();
    }

    public void RefreshLight()
    {
        currentLight = maxLight;
    }

    public bool CanPay(int amount)
    {
        return currentLight >= amount;
    }

    public void SpendLight(int amount)
    {
        currentLight -= amount;
    }

    public void RollSpeedDice()
    {
        foreach (var die in speedDice)
            die.Roll();
    }

    public SpeedDie GetHighestAvailableDie()
    {
        SpeedDie best = null;

        foreach (var die in speedDice)
        {
            if (die.used)
                continue;

            if (best == null || die.value > best.value)
                best = die;
        }

        return best;
    }

    public void TakeDamage(
        int amount,
        DamageType type)
    {
        int final =
            DamageCalculator.Calculate(
                amount,
                type,
                this
            );

        hp -= final;

        Debug.Log($"{unitName} took {final}");
    }

    public IEnumerator MoveTo(
        Vector3 target,
        float duration = 0.2f)
    {
        Vector3 start = visual.position;

        float t = 0;

        while (t < duration)
        {
            visual.position =
                Vector3.Lerp(
                    start,
                    target,
                    t / duration
                );

            t += Time.deltaTime;
            yield return null;
        }

        visual.position = target;
    }

    public IEnumerator Recoil(
        Vector3 dir,
        float distance,
        float duration)
    {
        Vector3 start = visual.position;
        Vector3 target =
            start + dir * distance;

        float t = 0;

        while (t < duration)
        {
            visual.position =
                Vector3.Lerp(
                    start,
                    target,
                    t / duration
                );

            t += Time.deltaTime;
            yield return null;
        }
    }

    public void ResetPosition()
    {
        visual.position = startPos;
    }

    public Vector3 GetClashPosition()
    {
        return clashAnchor.position;
    }

    public void PlayAttack()
    {
        if (attack != null)
            sr.sprite = attack;
    }

    public void PlayHit()
    {
        if (hit != null)
            sr.sprite = hit;
    }

    public void PlayWindup()
    {
        if (windup != null)
            sr.sprite = windup;
    }

    public void HideSpeed()
    {
        foreach (var die in speedDice)
        {
            if (die.ui != null)
                die.ui.Hide();
        }
    }

    public void ShowSpeed()
    {
        foreach (var die in speedDice)
        {
            if (die.ui != null)
                die.ui.Show();
        }
    }
}