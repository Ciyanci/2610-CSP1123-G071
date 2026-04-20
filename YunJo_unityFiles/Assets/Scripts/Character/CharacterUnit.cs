using UnityEngine;
using System.Collections;

public class CharacterUnit : MonoBehaviour
{
    public string unitName;
    public int hp;

    public enum UnitState
    {
        Idle,
        Moving,
        Windup,
        Attacking,
        Hit,
        Clashing
    }

    public UnitState currentState;

    public DiceRoller diceUI;
    public int currentSpeedRoll;

    public int maxEnergy = 5;
    public int currentEnergy;

    public bool isHighlighted;

    public void Highlight(bool state)
    {
        isHighlighted = state;
        sr.color = state ? Color.yellow : Color.white;
    }

    public Transform visual;
    public Transform headAnchor;
    public Transform clashAnchor;
    public Transform weaponAnchor;

    public Sprite idle;
    public Sprite move;
    public Sprite windup;
    public Sprite attack;
    public Sprite hit;

    public int currentSpeed;

    bool animLock;

    public UnitType unitType;

    [SerializeField] private SpriteRenderer sr;

    public float HalfWidth => sr.bounds.extents.x;

    Coroutine animRoutine;

    Vector3 startPos;
    Vector3 smoothHeadPos;

    void Awake()
    {
        sr = visual.GetComponent<SpriteRenderer>();
        startPos = visual.position;
        sr.sprite = idle;
        smoothHeadPos = headAnchor.position;
    }

    void Update()
    {
        smoothHeadPos = Vector3.Lerp(
            smoothHeadPos,
            headAnchor.position,
            Time.deltaTime * 12f
        );
    }

    public Vector3 GetSmoothedHead() => smoothHeadPos;

    public IEnumerator SetState(UnitState state)
    {
        currentState = state;
        yield return null;
    }

    public IEnumerator MoveTo(Vector3 target, float t = 0.2f)
    {
        currentState = UnitState.Moving;

        Vector3 start = visual.position;
        float time = 0;

        while (time < t)
        {
            visual.position = Vector3.Lerp(start, target, time / t);
            time += Time.deltaTime;
            yield return null;
        }

        visual.position = target;
        currentState = UnitState.Idle;
    }

    public IEnumerator WindUp(float duration)
    {
        currentState = UnitState.Windup;
        sr.sprite = windup;
        yield return new WaitForSeconds(duration);
    }

    public void PlayAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        currentState = UnitState.Attacking;
        sr.sprite = attack;
        yield return new WaitForSeconds(0.2f);
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
        sr.sprite = hit;
        yield return new WaitForSeconds(0.2f);
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

    void OnMouseDown()
    {
        CombatInputController.Instance.SelectUnit(this);
        Debug.Log("Unit clicked: " + name);
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
    }
}
//oh my god bro kill me right now