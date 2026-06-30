using UnityEngine;
using System.Collections;

public class BattlegroundParallax : MonoBehaviour
{
    [Header("Background Layers — move during cinematic")]
    public ParallaxLayer[] layers;

    [Header("Transition")]
    public float          transitionDuration = 0.45f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Coroutine activeTransition;

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
        active = owner.StartCoroutine(
            MoveLayer(renderer.transform, target, duration, curve));
    }

    public void SetPlanning(MonoBehaviour owner, float duration, AnimationCurve curve)
    {
        if (renderer == null) return;
        Init();

        if (active != null) owner.StopCoroutine(active);
        active = owner.StartCoroutine(
            MoveLayer(renderer.transform, startPos, duration, curve));
    }

    void Init()
    {
        if (initialized) return;
        startPos    = renderer.transform.position;
        initialized = true;
    }

    IEnumerator MoveLayer(
        Transform t, Vector3 target, float duration, AnimationCurve curve)
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
