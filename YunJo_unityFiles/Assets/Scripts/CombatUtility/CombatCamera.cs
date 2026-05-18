using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatCamera : MonoBehaviour
{
    Camera cam;

    Vector3 defaultPos;
    float   defaultSize;

    [Header("Zoom Levels")]
    public float planningSize   = 100f;
    public float cinematicSize  = 80f;

    [Header("Ease")]
    public float easeSpeed = 3f;

    //target state
    Vector3 targetPos;
    float   targetSize;

    bool cinematic = false; //it says assigned by never used but this one is important trust me

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
        transform.position   = Vector3.Lerp(transform.position, targetPos,
                                   Time.deltaTime * easeSpeed);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize,
                                   Time.deltaTime * easeSpeed);
    }

    //planning mode
    public void SetPlanningView()
    {
        cinematic  = false;
        targetPos  = new Vector3(defaultPos.x, defaultPos.y, transform.position.z);
        targetSize = planningSize;
    }

    //cinematic mode
    public void SetCinematicView()
    {
        cinematic = true;

        if (ClashLane.Instance != null)
        {
            Vector3 focus = ClashLane.Instance.CameraFocus;
            targetPos = new Vector3(focus.x, focus.y, transform.position.z);
        }

        targetSize = cinematicSize;
    }

    //shake (gotta rework this tmr)
    public IEnumerator Shake(float intensity, float duration)
    {
        Vector3 origin = targetPos;
        float   t      = 0f;

        while (t < duration)
        {
            Vector2 offset = Random.insideUnitCircle * intensity;
            //offset
            transform.position = Vector3.Lerp(transform.position,
                targetPos + new Vector3(offset.x, offset.y, 0f),
                Time.deltaTime * 30f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    //made it so that play() routes through new system
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
                //in cinematic mode all of these just ensures focus on the lane
                SetCinematicView();
                yield return new WaitForSeconds(a.duration);
                break;

            case CameraActionType.Shake:
                yield return Shake(a.shakeIntensity, a.duration);
                break;

            case CameraActionType.MoveTo:
                targetPos = new Vector3(a.position.x, a.position.y, transform.position.z);
                yield return new WaitForSeconds(a.duration);
                break;
        }
    }
}
