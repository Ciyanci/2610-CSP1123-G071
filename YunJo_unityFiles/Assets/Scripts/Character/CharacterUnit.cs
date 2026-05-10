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
    public List<SpeedSlot> speedSlots = new();
    public SpeedSlotRowUI slotRowUI;

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

        if (slotRowUI != null)
            slotRowUI.Bind(this);
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

    public void RollSpeedSlots()
    {
        foreach (var slot in speedSlots)
            slot.Roll();

        SortSlots();
    }

    public void ResetSpeedSlots()
    {
        foreach (var slot in speedSlots)
        {
            slot.ResetTurn();
            slot.Roll(); // optional: reroll each turn
        }
    }

    public void SortSlots()
    {
        speedSlots.Sort((a, b) => b.value.CompareTo(a.value));
    }

    public void CommitAllSlots()
    {
        foreach (var slot in speedSlots)
        {
            slot.Commit();
        }
    }

    public bool isLockedInCombat;

    public SpeedSlot GetHighestAvailableSlot()
    {
        SpeedSlot best = null;

        foreach (var slot in speedSlots)
        {
            if (slot.state == SlotState.Executed || slot.state == SlotState.Committed)
                continue;

            if (best == null || slot.value > best.value)
                best = slot;
        }

        return best;
    }

    bool IsSlotAvailable(SpeedSlot slot)
    {
        return slot.state == SlotState.Empty;
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
        foreach (var slot in speedSlots)
        {
            if (slot.ui != null)
                slot.ui.Hide();
        }
    }

    public void ShowSpeed()
    {
        foreach (var slot in speedSlots)
        {
            if (slot.ui != null)
                slot.ui.Show();
        }
    }
}