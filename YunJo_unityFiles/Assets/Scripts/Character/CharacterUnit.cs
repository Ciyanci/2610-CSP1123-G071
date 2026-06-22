using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterUnit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName;
    public UnitType unitType;

    [Header("Unit Data — assign in Inspector")]
    public UnitData unitData;       // ✅ reference for keypage/card editing in prep
    public KeypageData equippedKeypage; // ✅ assigned in prep, null for leaders

    [Header("Stats")]
    public int maxHP      = 100;
    public int hp         = 100;
    public int maxStagger = 50;
    public int stagger    = 50;

    [Header("UI")]
    public CharacterStatusUI statusUI;
    public LightBarUI lightBarUI;

    [Header("State")]
    public UnitState state = UnitState.Normal;

    public bool IsDead      => state == UnitState.Dead;
    public bool IsStaggered => state == UnitState.Staggered;
    public bool CanAct      => state == UnitState.Normal;

    [Header("Resources")]
    public int maxLight     = 5;
    public int currentLight = 1;

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
    public Sprite move;

    [HideInInspector] public Vector3 startPos;
    public Vector3 GetStartPos() => startPos;

    [Header("Facing")]
    public bool startsOnLeft = true;
    int facingSign = 1;

    void Awake()
    {
        startPos   = visual.position;
        facingSign = startsOnLeft ? 1 : -1;
        ApplyFacing(facingSign);
        if (deck == null)
            deck = GetComponent<CharacterDeck>();
        if (slotRowUI != null)
            slotRowUI.Bind(this);
    }

    void Start()
    {
        ResetSpeedSlots();
        statusUI?.Bind(this);
        lightBarUI?.Bind(this);
    }

    // =========================
    // APPLY KEYPAGE
    // Called by BattleStarter after prep
    // =========================
    public void ApplyKeypage(KeypageData keypage)
    {
        equippedKeypage = keypage;

        if (unitData == null) return;

        maxHP      = unitData.GetMaxHP(keypage);
        hp         = maxHP;
        maxStagger = unitData.GetMaxStagger(keypage);
        stagger    = maxStagger;
        resistances = unitData.GetResistances(keypage);

        statusUI?.Refresh();
        CombatHUDController.Instance?.RefreshAll();
    }

    // =========================
    // FACING
    // =========================
    public void SetInitialFacing(bool faceRight)
    {
        facingSign = faceRight ? 1 : -1;
        ApplyFacing(facingSign);
    }

    void ApplyFacing(int sign)
    {
        if (visual == null) return;
        Vector3 s = visual.localScale;
        s.x = Mathf.Abs(s.x) * sign;
        visual.localScale = s;
    }

    public void FaceTowardUnit(CharacterUnit other)
    {
        if (other == null || visual == null) return;
        int sign = other.visual.position.x > visual.position.x ? 1 : -1;
        ApplyFacing(sign);
    }

    public void RestoreDefaultFacing() => ApplyFacing(facingSign);

    // =========================
    // MOVEMENT
    // =========================
    public IEnumerator MoveTo(
        Vector3 target,
        float duration       = 0.2f,
        bool playMoveSprite  = true,
        CharacterUnit faceTarget = null)
    {
        Vector3 start = visual.position;
        if (Vector3.Distance(start, target) < 0.01f) yield break;

        if (faceTarget != null) FaceTowardUnit(faceTarget);
        else
        {
            int moveSign = target.x > start.x ? 1 : -1;
            ApplyFacing(moveSign);
        }

        if (playMoveSprite && move != null) sr.sprite = move;

        float t = 0;
        while (t < duration)
        {
            visual.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        visual.position = target;
        ApplyFacing(facingSign);
        if (playMoveSprite) sr.sprite = idle;
    }

    public IEnumerator Recoil(Vector3 dir, float distance, float duration)
    {
        Vector3 start  = visual.position;
        Vector3 target = start + dir * distance;
        float t = 0;
        while (t < duration)
        {
            visual.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        visual.position = target;
        ApplyFacing(facingSign);
    }

    public void ResetPosition()
    {
        visual.position = startPos;
        ApplyFacing(facingSign);
        sr.sprite = idle;
    }

    public Vector3 GetClashPosition() => clashAnchor.position;

    // =========================
    // LIGHT
    // =========================
    public void RefreshLight()
    {
        currentLight = maxLight;
        lightBarUI?.Refresh();
    }

    public bool CanPay(int amount)    => currentLight >= amount;
    public void SpendLight(int amount)
    {
        currentLight -= amount;
        lightBarUI?.Refresh();
    }

    // =========================
    // SPEED SLOTS
    // =========================
    public void RollSpeedSlots()
    {
        foreach (var slot in speedSlots) slot.Roll();
        SortSlots();
        if (slotRowUI != null && speedSlots.Count > 0) slotRowUI.Bind(this);
        slotRowUI?.AnimateRolls();
    }

    void OnTransformChildrenChanged()
    {
        if (slotRowUI == null) return;
        slotRowUI.Bind(this);
    }

    public void ResetSpeedSlots()
    {
        if (state == UnitState.Staggered)
        {
            stagger = maxStagger;
            state   = UnitState.Normal;
        }
        isInterrupted = false;
        InitializeSpeedSlots();
        RollSpeedSlots();
        defensiveDice.Clear();
        slotRowUI?.Refresh();
    }

    public void InitializeSpeedSlots()
    {
        if (speedSlots == null) speedSlots = new List<SpeedSlot>();
        speedSlots.Clear();
        int diceCount = GetSpeedDiceCount();
        for (int i = 0; i < diceCount; i++)
            speedSlots.Add(new SpeedSlot { owner = this });
        SortSlots();
        if (slotRowUI != null) slotRowUI.Bind(this);
    }

    public int GetSpeedDiceCount()
    {
        int count = 1;
        if (maxHP >= 60)  count++;
        if (maxHP >= 120) count++;
        return Mathf.Clamp(count, 1, 4);
    }

    public void SortSlots()
    {
        speedSlots.Sort((a, b) => b.value.CompareTo(a.value));
    }

    public void CommitAllSlots()
    {
        foreach (var slot in speedSlots)
            if (slot.state == SlotState.Planned)
                slot.Commit();
    }

    public SpeedSlot GetHighestAvailableSlot()
    {
        if (!CanAct) return null;
        SpeedSlot best = null;
        foreach (var slot in speedSlots)
        {
            if (slot.state == SlotState.Executed ||
                slot.state == SlotState.Committed) continue;
            if (best == null || slot.value > best.value)
                best = slot;
        }
        return best;
    }

    public void ClearCombatAssignments()
    {
        foreach (var slot in speedSlots) slot.Clear();
    }

    public bool CanResolveAction() => !IsDead && !isInterrupted;

    // =========================
    // DEFENSIVE
    // =========================
    public DefensiveDie GetAvailableDefense()
    {
        if (IsDead || isInterrupted) return null;
        if (defensiveDice == null || defensiveDice.Count == 0) return null;
        return defensiveDice[0];
    }

    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(int amount, DamageType type)
    {
        if (IsDead) return;
        int final = DamageCalculator.Calculate(amount, type, this);
        hp = Mathf.Max(0, hp - final);
        Debug.Log($"{unitName} took {final} HP | remaining: {hp}/{maxHP}");
        RefreshAllUI();
        EvaluateState();
    }

    public void TakeStaggerDamage(int amount)
    {
        if (IsDead) return;
        stagger = Mathf.Max(0, stagger - amount);
        Debug.Log($"{unitName} stagger | remaining: {stagger}/{maxStagger}");
        RefreshAllUI();
        EvaluateState();
    }

    void RefreshAllUI()
    {
        statusUI?.Refresh();
        CombatHUDController.Instance?.RefreshAll();
    }

    public IEnumerator TakeDamageWithKnockback(
        int amount, DamageType type, Vector3 attackerDir, bool returnToStart = true)
    {
        if (IsDead) yield break;
        int final = DamageCalculator.Calculate(amount, type, this);
        hp = Mathf.Max(0, hp - final);
        Debug.Log($"{unitName} took {final} HP (knockback) | remaining: {hp}/{maxHP}");
        RefreshAllUI();
        float knockDist = Mathf.Clamp(final * 1f, 3f, 12f);
        yield return Recoil(attackerDir, knockDist, 0.06f);
        if (returnToStart && !IsDead)
            yield return MoveTo(startPos, 0.2f);
        EvaluateState();
    }

    // =========================
    // STATE
    // =========================
    public void EvaluateState()
    {
        if (hp <= 0)      { Die();     return; }
        if (stagger <= 0) { Stagger(); return; }
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
        if (state == UnitState.Dead || state == UnitState.Staggered) return;
        state         = UnitState.Staggered;
        isInterrupted = true;
        Debug.Log($"{unitName} staggered (INTERRUPT)");
        ClearCombatAssignments();
    }

    public void Die()
    {
        if (state == UnitState.Dead) return;
        state = UnitState.Dead;
        Debug.Log($"{unitName} died");
        ClearCombatAssignments();
        gameObject.SetActive(false);
    }

    // =========================
    // SPRITES
    // =========================
    public void PlayAttack() => sr.sprite = attack;
    public void PlayHit()    => sr.sprite = hit;
    public void PlayWindup() => sr.sprite = windup;
    public void PlayMove()   => sr.sprite = move;

    // =========================
    // SPEED UI
    // =========================
    public void HideSpeed() { foreach (var s in speedSlots) s.ui?.Hide(); }
    public void ShowSpeed() { foreach (var s in speedSlots) s.ui?.Show(); }

    // =========================
    // INPUT
    // =========================
    void OnMouseDown()
    {
        if (CombatFlowController.Instance.IsTargeting)
        {
            CombatFlowController.Instance.ConfirmTarget(this);
            return;
        }
        CombatFlowController.Instance.SelectUnit(this);
    }

    public void SetInvolved(bool involved)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = involved ? 1f : 0.5f;
        sr.color = c;
    }

    public void ResetTransparency()
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;
    }
}
