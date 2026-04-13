using UnityEngine;
using System.Collections;

public class CombatCamera : MonoBehaviour
{
    public Camera cam;

    public float normalSize = 10.5f;
    public float clashSize = 11.5f;

    public float followStrength = 0.12f;
    public float horizontalClamp = 2.0f;
    public float verticalBias = -1.2f;

    public Vector3 basePos;

    void Awake()
    {
        basePos = cam.transform.position;
    }

    Vector3 ClampMid(Vector3 mid)
    {
        mid.y = Mathf.Clamp(mid.y, -3f, 3f);
        return mid;
    }

    public IEnumerator ClashZoom(Vector3 mid)
    {
        float t = 0;
        float dur = 0.25f;

        mid = ClampMid(mid);

        Vector3 start = cam.transform.position;

        Vector3 offset = new Vector3(
            Mathf.Clamp(mid.x * followStrength, -horizontalClamp, horizontalClamp),
            verticalBias,
            0
        );

        Vector3 target = basePos + offset;
        target.z = basePos.z;

        while (t < dur)
        {
            float e = Mathf.SmoothStep(0, 1, t / dur);

            cam.transform.position = Vector3.Lerp(start, target, e);
            cam.orthographicSize = Mathf.Lerp(normalSize, clashSize, e);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ImpactShake(float intensity, float time)
    {
        Vector3 start = cam.transform.position;
        float t = 0;

        while (t < time)
        {
            cam.transform.position = start + (Vector3)Random.insideUnitCircle * intensity;

            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = start;
    }

    public IEnumerator Reset()
    {
        float t = 0;
        float dur = 0.2f;

        Vector3 start = cam.transform.position;

        while (t < dur)
        {
            float e = Mathf.SmoothStep(0, 1, t / dur);

            cam.transform.position = Vector3.Lerp(start, basePos, e);
            cam.orthographicSize = Mathf.Lerp(clashSize, normalSize, e);

            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = basePos;
        cam.orthographicSize = normalSize;
    }
}
