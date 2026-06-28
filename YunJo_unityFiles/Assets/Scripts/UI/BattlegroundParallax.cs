using UnityEngine;
using System.Collections;

public class BattlegroundParallax : MonoBehaviour
{
    [Header("Floor Layer")]
    public SpriteRenderer floorRenderer;

    [Header("Planning State")]
    public float planningScaleY  = 1f;
    public float planningOffsetY = 0f;

    [Header("Cinematic State")]
    public float cinematicScaleY  = 0.7f;
    public float cinematicOffsetY = 20f;

    [Header("Transition")]
    public float          transitionDuration = 0.45f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Depth Layers")]
    public ParallaxLayer[] layers;

    Vector3    floorStartScale;
    Vector3    floorStartPos;
    Coroutine  activeTransition;

    void Awake()
    {
        if (floorRenderer == null) return;
        floorStartScale = floorRenderer.transform.localScale;
        floorStartPos   = floorRenderer.transform.position;
    }

    public void EnterCinematic()
    {
        if (activeTransition != null) StopCoroutine(activeTransition);
        activeTransition = StartCoroutine(
            TransitionFloor(cinematicScaleY, cinematicOffsetY));

        foreach (var layer in layers)
            layer.SetCinematic(this, transitionDuration, easeCurve);
    }

    public void ExitCinematic()
    {
        if (activeTransition != null) StopCoroutine(activeTransition);
        activeTransition = StartCoroutine(
            TransitionFloor(planningScaleY, planningOffsetY));

        foreach (var layer in layers)
            layer.SetPlanning(this, transitionDuration, easeCurve);
    }

    IEnumerator TransitionFloor(float targetScaleY, float targetOffsetY)
    {
        if (floorRenderer == null) yield break;

        Transform t = floorRenderer.transform;

        Vector3 fromScale = t.localScale;
        Vector3 toScale   = new Vector3(
            floorStartScale.x,
            floorStartScale.y * targetScaleY,
            floorStartScale.z);

        Vector3 fromPos = t.position;
        Vector3 toPos   = new Vector3(
            floorStartPos.x,
            floorStartPos.y + targetOffsetY,
            floorStartPos.z);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float eased = easeCurve.Evaluate(
                Mathf.Clamp01(elapsed / transitionDuration));

            t.localScale = Vector3.Lerp(fromScale, toScale, eased);
            t.position   = Vector3.Lerp(fromPos,  toPos,   eased);

            yield return null;
        }

        t.localScale = toScale;
        t.position   = toPos;
    }
}


[System.Serializable]
public class ParallaxLayer
{
    public SpriteRenderer renderer;

    [Range(0f, 1f)]
    public float depthFactor = 0.5f;

    public float cinematicOffsetY = -0.5f;
    public float cinematicOffsetX = 0f;

    Vector3   startPos;
    bool      initialized = false;
    Coroutine active;

    public void SetCinematic(MonoBehaviour owner, float duration, AnimationCurve curve)
    {
        if (renderer == null) return;
        Init();

        Vector3 target = startPos + new Vector3(
            cinematicOffsetX * depthFactor,
            cinematicOffsetY * depthFactor,
            0f);

        if (active != null) owner.StopCoroutine(active);
        active = owner.StartCoroutine(MoveLayer(renderer.transform, target, duration, curve));
    }

    public void SetPlanning(MonoBehaviour owner, float duration, AnimationCurve curve)
    {
        if (renderer == null) return;
        Init();

        if (active != null) owner.StopCoroutine(active);
        active = owner.StartCoroutine(MoveLayer(renderer.transform, startPos, duration, curve));
    }

    void Init()
    {
        if (initialized) return;
        startPos    = renderer.transform.position;
        initialized = true;
    }

    IEnumerator MoveLayer(Transform t, Vector3 target, float duration, AnimationCurve curve)
    {
        Vector3 from    = t.position;
        float   elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed    += Time.deltaTime;
            float eased = curve.Evaluate(Mathf.Clamp01(elapsed / duration));
            t.position  = Vector3.Lerp(from, target, eased);
            yield return null;
        }

        t.position = target;
    }
}
