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

    [Header("State")]
    public UnitState state = UnitState.Normal;

    public bool IsDead => state == UnitState.Dead;
    public bool IsStaggered => state == UnitState.Staggered;
    public bool CanAct => state == UnitState.Normal;

    [Header("Resources")]
    public int maxLight = 3;
    public int currentLight = 3;

    [Header("Resistances")]
    public DamageResistance resistances;

    [Header("Defensive Dice")]
    public List<DefensiveDie> defensiveDice = new();

    [Header("Deck")]
    public CharacterDeck deck;

    [Header("Speed Dice")]
    public List<SpeedSlot> speedSlots = new();
    public SpeedSlotRowUI slotRowUI;

    [Header("Combat Control")]
    public bool isInterrupted;

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

    void Start()
    {
        ResetSpeedSlots();
    }

    public void InitializeSpeedSlots()
    {
        if (speedSlots == null)
            speedSlots = new List<SpeedSlot>();

        speedSlots.Clear();

        int diceCount = GetSpeedDiceCount();

        for (int i = 0; i < diceCount; i++)
        {
            SpeedSlot slot = new SpeedSlot
            {
                owner = this
            };

            speedSlots.Add(slot);
        }

        SortSlots();

        if (slotRowUI != null)
            slotRowUI.Bind(this);
    }

    void Awake()
    {
        startPos = visual.position;

        if (deck == null)
            deck = GetComponent<CharacterDeck>();

        if (slotRowUI != null)
            slotRowUI.Bind(this);
    }

    // =========================
    // LIGHT SYSTEM
    // =========================
    public void RefreshLight() => currentLight = maxLight;

    public bool CanPay(int amount) => currentLight >= amount;

    public void SpendLight(int amount) => currentLight -= amount;

    // =========================
    // SPEED SLOTS
    // =========================
    public void RollSpeedSlots()
    {
        foreach (var slot in speedSlots)
            slot.Roll();

        SortSlots();

        // 🔥 CRITICAL: update UI immediately
        if (slotRowUI != null)
            slotRowUI.Refresh();
    }

    public void ResetSpeedSlots()
    {
        if (state == UnitState.Staggered)
        {
            stagger = maxStagger;
            state = UnitState.Normal;
        }

        isInterrupted = false;

        // Rebuild slots fresh every turn
        InitializeSpeedSlots();

        // Roll immediately
        RollSpeedSlots();

        defensiveDice.Clear();

        slotRowUI?.Refresh();
    }

    public int GetSpeedDiceCount()
    {
        int count = 1;

        // example progression thresholds
        if (maxHP >= 60)
            count++;

        if (maxHP >= 120)
            count++;

        //clamp because clamping is cool
        return Mathf.Clamp(count, 1, 4);
    }

    public void SortSlots()
    {
        speedSlots.Sort((a, b) => b.value.CompareTo(a.value));
    }

    public void CommitAllSlots()
    {
        foreach (var slot in speedSlots)
        {
            if (slot.state == SlotState.Planned)
                slot.Commit();
        }
    }

    public SpeedSlot GetHighestAvailableSlot()
    {
        if (!CanAct)
            return null;

        SpeedSlot best = null;

        foreach (var slot in speedSlots)
        {
            if (slot.state == SlotState.Executed ||
                slot.state == SlotState.Committed)
                continue;

            if (best == null || slot.value > best.value)
                best = slot;
        }

        return best;
    }

    public void ClearCombatAssignments()
    {
        foreach (var slot in speedSlots)
            slot.Clear();
    }

    public bool CanResolveAction()
    {
        if (IsDead)
            return false;

        if (isInterrupted)
            return false;

        return true;
    }

    //DEFENSIVE QUERY SYSTEM
    public DefensiveDie GetAvailableDefense()
    {
        if (IsDead || isInterrupted)
            return null;

        if (defensiveDice == null || defensiveDice.Count == 0)
            return null;

        return defensiveDice[0];
    }

    // =========================
    // DAMAGE SYSTEM
    // =========================
    public void TakeDamage(int amount, DamageType type)
    {
        if (IsDead) return;

        int final = DamageCalculator.Calculate(amount, type, this);

        hp -= final;

        Debug.Log($"{unitName} took {final} HP");

        EvaluateState();
    }

    public void TakeStaggerDamage(int amount)
    {
        if (IsDead) return;

        stagger -= amount;

        Debug.Log($"{unitName} took {amount} Stagger");

        EvaluateState();
    }

    // =========================
    // STATE RESOLUTION (CORE)
    // =========================
    public void EvaluateState()
    {
        if (hp <= 0)
        {
            Die();
            return;
        }

        if (stagger <= 0)
        {
            Stagger();
            return;
        }

        state = UnitState.Normal;
    }

    public void Stagger()
    {
        if (state == UnitState.Dead) return;

        state = UnitState.Staggered;

        Debug.Log($"{unitName} staggered");

        ClearCombatAssignments();
    }

    public void ApplyStagger()
    {
        if (state == UnitState.Dead)
            return;

        if (state == UnitState.Staggered)
            return;

        state = UnitState.Staggered;
        isInterrupted = true;

        Debug.Log($"{unitName} staggered (INTERRUPT)");

        ClearCombatAssignments();
    }

    public void Die()
    {
        if (state == UnitState.Dead)
            return;

        state = UnitState.Dead;

        Debug.Log($"{unitName} died");

        ClearCombatAssignments();

        gameObject.SetActive(false);
    }

    // =========================
    // VISUALS
    // =========================
    public IEnumerator MoveTo(Vector3 target, float duration = 0.2f)
    {
        Vector3 start = visual.position;
        float t = 0;

        while (t < duration)
        {
            visual.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        visual.position = target;
    }

    public IEnumerator Recoil(Vector3 dir, float distance, float duration)
    {
        Vector3 start = visual.position;
        Vector3 target = start + dir * distance;

        float t = 0;

        while (t < duration)
        {
            visual.position = Vector3.Lerp(start, target, t / duration);
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

    public void PlayAttack() => sr.sprite = attack;
    public void PlayHit() => sr.sprite = hit;
    public void PlayWindup() => sr.sprite = windup;

    public void HideSpeed()
    {
        foreach (var slot in speedSlots)
            slot.ui?.Hide();
    }

    public void ShowSpeed()
    {
        foreach (var slot in speedSlots)
            slot.ui?.Show();
    }

    void OnMouseDown()
    {
        if (CombatFlowController.Instance.IsTargeting)
        {
            CombatFlowController.Instance.ConfirmTarget(this);
            return;
        }
        CombatFlowController.Instance.SelectUnit(this);
    }
}