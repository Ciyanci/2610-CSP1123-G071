using UnityEngine;
using System.Collections;

public class CharacterUnit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName;
    public int hp;

    [Header("State")]
    public UnitState currentState;

    [Header("Visual Offset Fix")]
    public Vector3 headOffset = new Vector3(0, 1.2f, 0);

    public enum UnitState
    {
        Idle,
        Moving,
        Windup,
        Attacking,
        Hit,
        Clashing
    }

    [Header("Dice")]
    public DiceUI diceUI; // ✅ unified system
    public int currentSpeedRoll;

    public SpeedDiceUI speedDiceUI;

    [Header("Energy")]
    public int maxEnergy = 5;
    public int currentEnergy;

    

    public void RefreshEnergy()
    {
        currentEnergy = maxEnergy;
    }

    public bool CanPay(int cost)
    {
        return currentEnergy >= cost;
    }

    public void Spend(int cost)
    {
        currentEnergy -= cost;
    }

    public CharacterDeck deck;

    void OnMouseDown()
    {
        if (!CombatFlowController.Instance.inputEnabled)
            return;

        CombatFlowController.Instance.SelectUnit(this);
        Debug.Log($"[INPUT] Clicked unit: {name}");
    }

    [Header("Highlight")]
    public bool isHighlighted;

    public void Highlight(bool state)
    {
        isHighlighted = state;

        if (sr != null)
            sr.color = state ? Color.yellow : Color.white;
    }

    [Header("Transforms")]
    public Transform visual;
    public Transform headAnchor;
    public Transform clashAnchor;
    public Transform weaponAnchor;

    [Header("Sprites")]
    public Sprite idle;
    public Sprite move;
    public Sprite windup;
    public Sprite attack;
    public Sprite hit;

    [Header("Combat")]
    public int currentSpeed;
    public UnitType unitType;

    [SerializeField] private SpriteRenderer sr;

    public float HalfWidth => sr != null ? sr.bounds.extents.x : 0.5f;

    Vector3 smoothHeadPos;
    Vector3 combatStartPos;

    public void SetCombatStartPosition()
    {
        combatStartPos = visual.position;
    }
    // -----------------------------
    // ✅ CLASH POSITION (FIXED ERROR)
    // -----------------------------
    public Vector3 GetClashPosition()
    {
        if (clashAnchor != null)
        {
            Vector3 pos = clashAnchor.position;
            pos.y = -7.5f;
            return pos;
        }

        return transform.position;
    }

// -----------------------------
// POSITION RESET (RESTORED)
// -----------------------------
    Vector3 startWorldPos;
    public void ResetPosition()
    {
        StopAllCoroutines();

        if (visual != null)
            visual.position = startWorldPos;

        currentState = UnitState.Idle;

        if (sr != null && idle != null)
            sr.sprite = idle;
    }

    public Vector3 GetCombatFocusPoint()
    {
        return visual != null
            ? visual.position + new Vector3(0, 1.0f, 0)
            : transform.position;
    }

    public void PlayWindup()
    {
        Debug.Log($"{name} windup");

        // if using animator:
        // animator.SetTrigger("Windup");
    }

    void Awake()
    {
        sr = visual.GetComponent<SpriteRenderer>();
        startWorldPos = visual.position; // IMPORTANT ADD
        sr.sprite = idle;
        smoothHeadPos = headAnchor.position;

        if (visual != null)
            sr = visual.GetComponent<SpriteRenderer>();

        if (sr != null && idle != null)
            sr.sprite = idle;

        if (headAnchor != null)
            smoothHeadPos = headAnchor.position;
    }

    void Start()
    {
        UnitRegistry.Instance?.Refresh();
    }

    void Update()
    {
        if (headAnchor == null) return;

        smoothHeadPos = Vector3.Lerp(
            smoothHeadPos,
            headAnchor.position,
            Time.deltaTime * 12f
        );
    }

    public Vector3 GetDiceAnchor()
    {
        return headAnchor != null ? headAnchor.position + headOffset : transform.position + Vector3.up * 1.2f;
    }

    //movement if u wanna change then change here
    public IEnumerator MoveTo(Vector3 target, float t = 0.2f)
    {
        currentState = UnitState.Moving;

        Vector3 start = visual.position;
        float time = 0;

        while (time < t)
        {
            visual.position = Vector3.Lerp(start, target, time / t);
            time += Time.deltaTime;
            if (sr != null && move != null)
            sr.sprite = move;
            yield return null;
        }

        visual.position = target;
        currentState = UnitState.Idle;
    }

    public IEnumerator WindUp(float duration)
    {
        currentState = UnitState.Windup;

        if (sr != null && windup != null)
            sr.sprite = windup;

        yield return new WaitForSeconds(duration);
    }

    //animation but temu
    public void PlayAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        currentState = UnitState.Attacking;

        if (sr != null && attack != null)
            sr.sprite = attack;

        yield return new WaitForSeconds(0.2f);

        if (sr != null && idle != null)
            sr.sprite = idle;

        currentState = UnitState.Idle;
    }

    public void PlayHit()
    {
        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        currentState = UnitState.Hit;

        if (sr != null && hit != null)
            sr.sprite = hit;

        yield return new WaitForSeconds(0.2f);

        if (sr != null && idle != null)
            sr.sprite = idle;

        currentState = UnitState.Idle;
    }

    public IEnumerator Recoil(Vector3 direction, float strength, float duration)
    {
        Vector3 start = visual.position;
        Vector3 target = start + direction.normalized * strength;

        float t = 0;
        while (t < duration)
        {
            visual.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < duration)
        {
            visual.position = Vector3.Lerp(target, start, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void RollSpeed()
    {
        currentSpeedRoll = Random.Range(1, 10);

        if (speedDiceUI != null)
        {
            speedDiceUI.Init(headAnchor);
            speedDiceUI.SetValue(currentSpeedRoll);
            speedDiceUI.Show();
        }
    }

    public void HideSpeed()
    {
        speedDiceUI?.Hide();
    }

    public void ShowSpeed()
    {
        speedDiceUI?.Show();
    }

    //damage, modify how they take damage here
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
    }
}
//oh my god bro kill me right now