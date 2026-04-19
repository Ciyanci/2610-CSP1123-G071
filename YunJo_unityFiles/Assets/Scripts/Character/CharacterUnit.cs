using UnityEngine;
using System.Collections;
public enum UnitType
{
    Melee,
    Ranged
}

public class CharacterUnit : MonoBehaviour
{
    public string unitName;
    public int hp;

    public Transform visual;
    public Transform headAnchor;
    public Transform clashAnchor;
    public Transform weaponAnchor;

    public Sprite idle;
    public Sprite move;
    public Sprite windup;
    public Sprite attack;
    public Sprite hit;

    bool animLock;
    public UnitType unitType;

    public Vector3 GetClashPosition()
    {
        // force same Y axis (LoR style)
        Vector3 pos = clashAnchor.position;
        pos.y = 0f;
        return pos;
    }

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

        smoothHeadPos = headAnchor != null ? headAnchor.position : Vector3.zero;
    }

    void Update()
    {
        if (headAnchor != null)
        {
            smoothHeadPos = Vector3.Lerp(
                smoothHeadPos,
                headAnchor.position,
                Time.deltaTime * 12f
            );
        }
    }

    public Vector3 GetSmoothedHead()
    {
        return smoothHeadPos;
    }

    public void ResetState()
    {
        sr.sprite = idle;
        visual.position = startPos;
    }

    public IEnumerator MoveTo(Vector3 target, float t = 0.2f)
    {
        if (!animLock) sr.sprite = move;

        Vector3 start = visual.position;
        float time = 0;

        while (time < t)
        {
            visual.position = Vector3.Lerp(start, target, time / t);
            time += Time.deltaTime;
            yield return null;
        }

        visual.position = target;
    }

    public IEnumerator WindUp(float duration)
    {
        sr.sprite = windup;
        yield return new WaitForSeconds(duration);
    }

    public void PlayAttack() => PlayAnim(attack, 0.2f);
    public void PlayHit() => PlayAnim(hit, 0.2f);

    void PlayAnim(Sprite s, float d)
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(AnimLocked(s, d));
    }

    IEnumerator AnimLocked(Sprite s, float d)
    {
        animLock = true;
        sr.sprite = s;
        yield return new WaitForSeconds(d);
        animLock = false;
        sr.sprite = idle;
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

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
    }
}
//oh my god bro kill me right now