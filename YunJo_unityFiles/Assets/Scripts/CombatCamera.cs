using UnityEngine;
using System.Collections;

public class CombatCamera : MonoBehaviour
{
    public Camera cam;
    public float normalSize = 15;
    public float clashSize = 6f;
    public float followStrength = 0.35f;
    public float horizontalClamp = 2.5f;
    public float verticalOffset = 0.8f;
    public Vector3 basePos;

    void Awake()
    {
        basePos = cam.transform.position;
        basePos.y = 0f;
    }

    public IEnumerator LeanToward(Vector3 winnerPos)
    {
        float t = 0;
        float dur = 0.2f;
        Vector3 start = cam.transform.position;

        Vector3 offset = new Vector3(
            Mathf.Clamp(winnerPos.x * followStrength, -horizontalClamp, horizontalClamp),
            0f,
            0
        );

        Vector3 target = start + offset;

        while (t < dur)
        {
            float eased = Mathf.SmoothStep(0,1,t / dur);

            cam.transform.position = Vector3.Lerp(start, target, eased);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ImpactBurst()
    {
        float t = 0;
        float dur = 0.12f;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        Vector3 punch = startPos + new Vector3(0, -0.15f, 0);

        while(t < dur)
        {
            float eased = Mathf.SmoothStep(0,1, t / dur);

            cam.transform.position = Vector3.Lerp(startPos, punch, eased);
            cam.orthographicSize = Mathf.Lerp(startSize, startSize - 0.4f, eased);

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.transform.position = startPos;
        cam.orthographicSize = startSize;

    }

    public IEnumerator ClashZoom(Vector3 mid)
    {
        float t = 0;
        float dur = 0.25f;

        Vector3 startPos = cam.transform.position;
        Vector3 offset = new Vector3(
            Mathf.Clamp(mid.x * followStrength, -horizontalClamp, horizontalClamp),
            verticalOffset,
            0
        );

        Vector3 targetPos = basePos + offset;
        targetPos.z = basePos.z;

        while (t < dur)
        {
            float eased = Mathf.SmoothStep(0,1, t / dur);
            cam.transform.position = Vector3.Lerp(startPos,targetPos,eased);
            cam.orthographicSize = Mathf.Lerp(normalSize, clashSize, eased);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Reset()
    {
        float t = 0;
        float dur = 0.2f;

        Vector3 startPos = cam.transform.position;

        while (t < dur)
        {
            float eased = Mathf.SmoothStep(0,1, t / dur);
            cam.transform.position = Vector3.Lerp(startPos, basePos, eased);
            cam.orthographicSize = Mathf.Lerp(clashSize, normalSize, eased);

            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = basePos;
        cam.orthographicSize = normalSize;
    }
}
