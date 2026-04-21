using UnityEngine;
using System.Collections;

public class CombatCamera : MonoBehaviour
{
    public Camera cam;

    public float normalSize = 25f;
    public float clashSize = 11.5f;

    public Vector3 basePos;

    void Awake()
    {
        basePos = cam.transform.position;
    }

    public IEnumerator ClashZoom(Vector3 focusPoint)
    {
        float t = 0;
        float dur = 0.2f;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        Vector3 targetPos = focusPoint + new Vector3(0, -1.0f, 0);
        targetPos.z = basePos.z;

        while (t < dur)
        {
            float e = Mathf.SmoothStep(0, 1, t / dur);

            cam.transform.position = Vector3.Lerp(startPos, targetPos, e);
            cam.orthographicSize = Mathf.Lerp(startSize, clashSize, e);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Reset()
    {
        float t = 0;
        float dur = 0.25f;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        while (t < dur)
        {
            float e = Mathf.SmoothStep(0, 1, t / dur);

            cam.transform.position = Vector3.Lerp(startPos, basePos, e);
            cam.orthographicSize = Mathf.Lerp(startSize, normalSize, e);

            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = basePos;
        cam.orthographicSize = normalSize;
    }
}