using UnityEngine;
using System.Collections;

public class CombatCamera : MonoBehaviour
{
    public float zoomSize = 15f;
    public float moveSpeed = 2f;

    Camera cam;
    Vector3 defaultPos;
    float defaultSize;

    Coroutine followRoutine;

    void Awake()
    {
        cam = Camera.main;
        defaultPos = transform.position;
        defaultSize = cam.orthographicSize;
    }

    // -----------------------------
    // BASIC ZOOM (used before rolls)
    // -----------------------------
    public IEnumerator ClashZoom(Vector3 focus)
    {
        Vector3 targetPos = new Vector3(focus.x, focus.y, transform.position.z);

        float t = 0;
        float dur = 0.25f;

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        while (t < dur)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t / dur);
            cam.orthographicSize = Mathf.Lerp(startSize, zoomSize, t / dur);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = zoomSize;
    }

    // -----------------------------
    // FOLLOW TARGET (LOSER TRACK)
    // -----------------------------
    public void FollowTarget(Transform target, float duration)
    {
        if (followRoutine != null)
            StopCoroutine(followRoutine);

        followRoutine = StartCoroutine(FollowRoutine(target, duration));
    }

    public IEnumerator ClashCenter(Vector3 point)
    {
        Vector3 target = new Vector3(point.x, point.y, transform.position.z);

        float t = 0;
        float dur = 0.35f; // slightly longer = smoother

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        while (t < 1f)
        {
            t += Time.deltaTime / dur;

            // 🔥 SMOOTHSTEP easing (ease in + ease out)
            float eased = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPos, target, eased);
            cam.orthographicSize = Mathf.Lerp(startSize, zoomSize, eased);

            yield return null;
        }

        // no hard snap needed anymore, but keep for precision
        transform.position = target;
        cam.orthographicSize = zoomSize;
    }

    IEnumerator FollowRoutine(Transform target, float duration)
    {
        float t = 0;

        while (t < duration && target != null)
        {
            Vector3 targetPos = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * moveSpeed
            );

            t += Time.deltaTime;
            yield return null;
        }
    }

    // -----------------------------
    // SCREENSHAKE (FOR TIE)
    // -----------------------------
    public IEnumerator Shake(float intensity, float duration)
    {
        Vector3 original = transform.position;

        float t = 0;

        while (t < duration)
        {
            Vector3 offset = Random.insideUnitCircle * intensity;
            transform.position = original + new Vector3(offset.x, offset.y, 0);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = original;
    }

    // -----------------------------
    // RESET
    // -----------------------------
    public IEnumerator Reset()
    {
        if (followRoutine != null)
            StopCoroutine(followRoutine);

        float t = 0;
        float dur = 0.3f;

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        while (t < dur)
        {
            transform.position = Vector3.Lerp(startPos, defaultPos, t / dur);
            cam.orthographicSize = Mathf.Lerp(startSize, defaultSize, t / dur);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = defaultPos;
        cam.orthographicSize = defaultSize;
    }
    public IEnumerator SmoothFollow(Transform target, float duration)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(target.position.x, target.position.y, transform.position.z);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float eased = t * t * (3f - 2f * t); // smoothstep

            transform.position = Vector3.Lerp(start, end, eased);

            yield return null;
        }
    }
}