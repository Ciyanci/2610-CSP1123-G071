using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatCamera : MonoBehaviour
{
    Camera cam;

    Vector3 defaultPos;
    float   defaultSize;

    [Header("Zoom Levels")]
    public float planningSize     = 80f;
    public float cinematicSize    = 10f;
    public float minCinematicSize = 10f;
    public float maxCinematicSize = 40f;

    [Header("Framing")]
    public float framePadding = 8f;

    [Header("Follow — position lag")]
    //higher = snappier, lower = more lag/trail
    public float positionSmoothTime  = 0.35f;

    //extra lag multiplier applied during active resolution
    public float combatLagMultiplier = 2.2f;

    [Header("Follow — zoom lag")]
    public float zoomSmoothTime = 0.5f;

    //internal smoothdamp state
    Vector3 currentVelocity  = Vector3.zero;
    float   currentZoomVel   = 0f;

    Vector3 targetPos;
    float   targetSize;

    //set true during a resolution so lag multiplier kicks in
    bool inResolution = false;

    Coroutine trackingCoroutine;

    void Awake()
    {
        cam         = Camera.main;
        defaultPos  = transform.position;
        defaultSize = planningSize;

        cam.orthographicSize = planningSize;

        targetPos  = defaultPos;
        targetSize = planningSize;
    }

    void LateUpdate()
    {
        float smoothTime = inResolution
            ? positionSmoothTime * combatLagMultiplier
            : positionSmoothTime;

        //SmoothDamp — has velocity so it trails naturally
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            smoothTime);

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetSize,
            ref currentZoomVel,
            zoomSmoothTime);
    }

    //planning state (default)
    public void SetPlanningView()
    {
        inResolution = false;
        targetPos    = new Vector3(defaultPos.x, defaultPos.y, transform.position.z);
        targetSize   = planningSize;
    }

    //frame two units
    public void FrameUnits(CharacterUnit a, CharacterUnit b)
    {
        inResolution = true;

        if (a == null && b == null) { SetCinematicView(); return; }

        Vector3 posA = a != null ? a.visual.position : b.visual.position;
        Vector3 posB = b != null ? b.visual.position : a.visual.position;

        Vector3 mid = (posA + posB) * 0.5f;
        targetPos   = new Vector3(mid.x, mid.y, transform.position.z);

        float dist     = Vector3.Distance(posA, posB);
        float ideal    = (dist * 0.5f) + framePadding;
        targetSize     = Mathf.Clamp(ideal, minCinematicSize, maxCinematicSize);
    }

    public void FrameUnits(CharacterUnit a) => FrameUnits(a, null);

    //cinematic (fallback ver)
    public void SetCinematicView()
    {
        inResolution = true;

        if (ClashLane.Instance != null)
        {
            Vector3 focus = ClashLane.Instance.CameraFocus;
            targetPos = new Vector3(focus.x, focus.y, transform.position.z);
        }
        targetSize = cinematicSize;
    }

    //unopposed tracking cam
    public void StartTracking(CharacterUnit unit, CharacterUnit target = null)
    {
        StopTracking();
        inResolution     = true;
        trackingCoroutine = StartCoroutine(TrackUnit(unit, target));
    }
    public void StopTracking()
    {
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
            trackingCoroutine = null;
        }
    }
    IEnumerator TrackUnit(CharacterUnit unit, CharacterUnit secondary = null)
    {
        while (unit != null && unit.gameObject.activeInHierarchy)
        {
            Vector3 posA = unit.visual != null
                ? unit.visual.position
                : unit.transform.position;
            if (secondary != null && secondary.gameObject.activeInHierarchy)
            {
                //frame both — midpoint shifts as aggressor moves
                Vector3 posB = secondary.visual != null
                    ? secondary.visual.position
                    : secondary.transform.position;
                Vector3 mid  = (posA + posB) * 0.5f;
                targetPos    = new Vector3(mid.x, mid.y, transform.position.z);
                float dist   = Vector3.Distance(posA, posB);
                float ideal  = (dist * 0.5f) + framePadding;
                targetSize   = Mathf.Clamp(ideal, minCinematicSize, maxCinematicSize);
            }
            else
            {
                //single unit — just follow them
                targetPos  = new Vector3(posA.x, posA.y, transform.position.z);
                targetSize = minCinematicSize;
            }
            yield return null;
        }
    }

    //shake
    public IEnumerator Shake(float intensity, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            Vector2 offset = Random.insideUnitCircle * intensity;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos + new Vector3(offset.x, offset.y, 0f),
                ref currentVelocity,
                0.03f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Play(List<CameraAction> sequence)
    {
        foreach (var action in sequence)
            yield return Execute(action);
    }

    IEnumerator Execute(CameraAction a)
    {
        switch (a.type)
        {
            case CameraActionType.Reset:
                SetPlanningView();
                yield return new WaitForSeconds(a.duration);
                break;

            case CameraActionType.FrameTargets:
            case CameraActionType.FocusTarget:
            case CameraActionType.Zoom:
                SetCinematicView();
                yield return new WaitForSeconds(a.duration);
                break;

            case CameraActionType.Shake:
                yield return Shake(a.shakeIntensity, a.duration);
                break;

            case CameraActionType.MoveTo:
                targetPos = new Vector3(
                    a.position.x, a.position.y, transform.position.z);
                yield return new WaitForSeconds(a.duration);
                break;
        }
    }
}