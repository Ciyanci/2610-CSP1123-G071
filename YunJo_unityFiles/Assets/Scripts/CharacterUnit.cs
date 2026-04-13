using UnityEngine;
using System.Collections;

public class CharacterUnit : MonoBehaviour
{
    public string unitName;
    public int hp;

    [Header("Scene refs")]
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

    bool animLock;

    [SerializeField] private SpriteRenderer sr;

    public float HalfWidth => sr.bounds.extents.x;

    Coroutine animRoutine;

    Vector3 startPos;

    void Awake()
    {
        sr = visual.GetComponent<SpriteRenderer>();
        startPos = transform.position;
        sr.sprite = idle;
    }

    public void ResetState()
    {
        sr.sprite = idle;
        transform.position = startPos;
    }

    public IEnumerator MoveTo(Vector3 target, float t = 0.2f)
    {
        if(!animLock)
            sr.sprite = move;

        Vector3 start = transform.position;
        float time = 0;

        while (time < t)
        {
            Vector3 pos = Vector3.Lerp(start,target,time / t);
            transform.position = pos;
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    public IEnumerator WindUp(float duration)
    {
        sr.sprite = windup;
        yield return new WaitForSeconds(duration);
    }

    public void PlayAttack()
    {
        PlayAnim(attack, 0.2f);
    }

    public void PlayHit()
    {
        PlayAnim(hit, 0.2f);
    }

    void PlayAnim(Sprite s, float d)
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimLocked(s,d));
    }

    IEnumerator AnimLocked(Sprite s, float d)
    {
        animLock = true;
        sr.sprite = s;
        yield return new WaitForSeconds(d);
        animLock = false;
        sr.sprite = idle;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
    }
}