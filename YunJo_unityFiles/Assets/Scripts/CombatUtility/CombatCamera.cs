using UnityEngine;
using System.Collections;

public class CombatCamera : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Zoom Levels (LoR Style Tuning)")]
    public float defaultZoom = 70f;
    public float clashZoom = 15f;
    public float focusZoom = 10f;
    public float unopposedZoom = 15f;

    Camera cam;

    Vector3 defaultPos;
    float defaultSize;

    Coroutine followRoutine;

    void Awake()
    {
        cam = Camera.main;

        defaultPos = transform.position;
        defaultSize = defaultZoom; // IMPORTANT: stable baseline
        cam.orthographicSize = defaultZoom;
    }

    // -----------------------------
    // CLASH ZOOM (center fight)
    // -----------------------------
    public IEnumerator ClashZoom(Vector3 focus)
    {
        Vector3 targetPos = new Vector3(focus.x, focus.y, transform.position.z);

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        float t = 0;
        float dur = 0.25f;

        while (t < dur)
        {
            float e = t / dur;
            e = e * e * (3f - 2f * e); // smoothstep

            transform.position = Vector3.Lerp(startPos, targetPos, e);
            cam.orthographicSize = Mathf.Lerp(startSize, clashZoom, e);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = clashZoom;
    }

    // -----------------------------
    // CENTER CLASH (stable framing)
    // -----------------------------
    public IEnumerator ClashCenter(Vector3 point)
    {
        Vector3 target = new Vector3(point.x, point.y, transform.position.z);

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        float t = 0;
        float dur = 0.35f;

        while (t < dur)
        {
            float e = t / dur;
            e = e * e * (3f - 2f * e);

            transform.position = Vector3.Lerp(startPos, target, e);
            cam.orthographicSize = Mathf.Lerp(startSize, clashZoom, e);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        cam.orthographicSize = clashZoom;
    }

    // -----------------------------
    // FOLLOW LOSER
    // -----------------------------
    public void FollowTarget(Transform target, float duration)
    {
        if (followRoutine != null)
            StopCoroutine(followRoutine);

        followRoutine = StartCoroutine(FollowRoutine(target, duration));
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
    // FOCUS (single unit / attack)
    // -----------------------------
    public IEnumerator Focus(Vector3 target, float zoom, float duration)
    {
        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        Vector3 targetPos = new Vector3(target.x, target.y, transform.position.z);

        float t = 0;

        while (t < duration)
        {
            float e = t / duration;
            e = e * e * (15f - 2f * e);

            transform.position = Vector3.Lerp(startPos, targetPos, e);
            cam.orthographicSize = Mathf.Lerp(startSize, zoom, e);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = zoom;
    }

    // -----------------------------
    // SHAKE (tie)
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
    // RESET CAMERA
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
            float e = t / dur;
            e = e * e * (3f - 2f * e);

            transform.position = Vector3.Lerp(startPos, defaultPos, e);
            cam.orthographicSize = Mathf.Lerp(startSize, defaultZoom, e);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = defaultPos;
        cam.orthographicSize = defaultZoom;
    }

    // -----------------------------
    // SMOOTH FOLLOW (optional utility)
    // -----------------------------
    public IEnumerator SmoothFollow(Transform target, float duration)
    {
        Vector3 start = transform.position;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float e = t * t * (3f - 2f * t);

            Vector3 end = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(start, end, e);

            yield return null;
        }
    }
}