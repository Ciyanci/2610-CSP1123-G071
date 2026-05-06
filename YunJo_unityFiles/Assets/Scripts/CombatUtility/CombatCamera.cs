using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatCamera : MonoBehaviour
{
    Camera cam;

    Vector3 defaultPos;
    float defaultSize;

    [Header("Zoom Offset")]
    [Tooltip("How much to zoom IN from default size")]
    public float zoomOffset = 10f;

    [Tooltip("Clamp so camera never over-zooms")]
    public float minSize = 40f;

    void Awake()
    {
        cam = Camera.main;
        defaultPos = transform.position;
        defaultSize = cam.orthographicSize;
    }

    // =========================
    // MAIN PIPELINE
    // =========================
    public IEnumerator Play(List<CameraAction> sequence)
    {
        foreach (var action in sequence)
        {
            yield return Execute(action);
        }
    }

    IEnumerator Execute(CameraAction a)
    {
        switch (a.type)
        {
            case CameraActionType.MoveTo:
                yield return MoveAndZoom(transform.position, cam.orthographicSize, a.position, cam.orthographicSize, a.duration);
                break;

            case CameraActionType.FocusTarget:
                if (a.target != null)
                {
                    yield return MoveAndZoom(
                        transform.position,
                        cam.orthographicSize,
                        a.target.position,
                        cam.orthographicSize,
                        a.duration
                    );
                }
                break;

            case CameraActionType.Zoom:
                yield return MoveAndZoom(
                    transform.position,
                    cam.orthographicSize,
                    transform.position,
                    GetZoom(a.zoom),
                    a.duration
                );
                break;

            case CameraActionType.Reset:
                yield return MoveAndZoom(
                    transform.position,
                    cam.orthographicSize,
                    defaultPos,
                    defaultSize,
                    a.duration <= 0 ? 0.3f : a.duration
                );
                break;

            case CameraActionType.Shake:
                yield return Shake(a.shakeIntensity, a.duration);
                break;

            case CameraActionType.FrameTargets:
                if (a.targets != null && a.targets.Count >= 2)
                {
                    yield return FrameTargets(a.targets, a.duration);
                }
                break;
        }
    }

    // =========================
    // CORE
    // =========================
    IEnumerator MoveAndZoom(
        Vector3 startPos,
        float startZoom,
        Vector3 targetPos,
        float targetZoom,
        float duration
    )
    {
        Vector3 endPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);

        float t = 0;

        while (t < duration)
        {
            float e = Ease(t / duration);

            transform.position = Vector3.Lerp(startPos, endPos, e);
            cam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, e);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        cam.orthographicSize = targetZoom;
    }

    // =========================
    // 🔥 FIXED ZOOM SYSTEM
    // =========================
    float GetZoom(float requestedOffset)
    {
        float target = defaultSize - requestedOffset;

        return Mathf.Clamp(target, minSize, defaultSize);
    }

    // =========================
    // AUTO FRAME (SAFE)
    // =========================
  IEnumerator FrameTargets(List<Transform> targets, float duration)
    {
        if (targets == null || targets.Count == 0)
            yield break;

        Vector3 center = Vector3.zero;

        foreach (var t in targets)
            center += t.position;

        center /= targets.Count;

        float maxDist = 0f;

        foreach (var t in targets)
        {
            float d = Vector3.Distance(center, t.position);
            if (d > maxDist) maxDist = d;
        }

        float screenPadding = 2.5f;

        float targetZoom = maxDist * screenPadding;

        // clamp so it doesn't go insane
        targetZoom = Mathf.Clamp(targetZoom, 15f, 70f);

        yield return MoveAndZoom(
            transform.position,
            cam.orthographicSize,
            center,
            targetZoom,
            duration
        );
}

    // =========================
    IEnumerator Shake(float intensity, float duration)
    {
        Vector3 original = transform.position;

        float t = 0;

        while (t < duration)
        {
            Vector2 offset = Random.insideUnitCircle * intensity;
            transform.position = original + new Vector3(offset.x, offset.y, 0);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = original;
    }

    float Ease(float t)
    {
        return t * t * (3f - 2f * t);
    }
}