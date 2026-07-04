using UnityEngine;
using System.Collections;

public class BattlegroundParallax : MonoBehaviour
{
    [Header("Background Layers — move during cinematic")]
    public ParallaxLayer[] layers;

    [Header("Transition")]
    public float transitionDuration = 0.45f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void EnterCinematic()
    {
        foreach (var layer in layers)
            layer.SetCinematic(this, transitionDuration, easeCurve);
    }

    public void ExitCinematic()
    {
        foreach (var layer in layers)
            layer.SetPlanning(this, transitionDuration, easeCurve);
    }
}

[System.Serializable]
public class ParallaxLayer
{
    public SpriteRenderer renderer;

    [Range(0f, 1f)]
    [Tooltip("0 = no movement, 1 = full offset applied")]
    public float depthFactor = 0.5f;

    [Tooltip("How far this layer drifts downward in cinematic mode (world units)")]
    public float cinematicOffsetY = -1.5f;

    [Tooltip("Optional horizontal drift")]
    public float cinematicOffsetX = 0f;

    [Header("(Y-axis zoom)")]
    [Tooltip("Y-scale multiplier in cinematic mode (e.g. 1.3 = 30% taller)")]
    public float scaleMultiplierY = 1f;

    Vector3   startPos;
    Vector3   startScale;
    bool      initialized;
    Coroutine active;

    public void SetCinematic(MonoBehaviour owner, float duration, AnimationCurve curve)
    {
        if (renderer == null) return;
        Init();

        Vector3 targetPos = startPos + new Vector3(
            cinematicOffsetX * depthFactor,
            cinematicOffsetY * depthFactor,
            0f);

        Vector3 targetScale = new Vector3(
            startScale.x,
            startScale.y * scaleMultiplierY,
            startScale.z);

        if (active != null) owner.StopCoroutine(active);
        active = owner.StartCoroutine(
            TransitionLayer(renderer.transform, targetPos, targetScale, duration, curve));
    }

    public void SetPlanning(MonoBehaviour owner, float duration, AnimationCurve curve)
    {
        if (renderer == null) return;
        Init();

        if (active != null) owner.StopCoroutine(active);
        active = owner.StartCoroutine(
            TransitionLayer(renderer.transform, startPos, startScale, duration, curve));
    }

    void Init()
    {
        if (initialized) return;
        startPos    = renderer.transform.position;
        startScale  = renderer.transform.localScale;
        initialized = true;
    }

    IEnumerator TransitionLayer(
        Transform t, Vector3 targetPos, Vector3 targetScale,
        float duration, AnimationCurve curve)
    {
        Vector3 fromPos   = t.position;
        Vector3 fromScale = t.localScale;
        float   elapsed   = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float eased = curve.Evaluate(Mathf.Clamp01(elapsed / duration));

            t.position   = Vector3.Lerp(fromPos,   targetPos,   eased);
            t.localScale = Vector3.Lerp(fromScale, targetScale, eased);

            yield return null;
        }

        t.position   = targetPos;
        t.localScale = targetScale;
    }
}
