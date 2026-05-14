using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CombatArrow : MonoBehaviour
{
    [Header("Curve")]
    public int   segments    = 40;
    public float arcHeight   = 2.5f;   // world units, how high the arc rises

    [Header("Tip")]
    public Transform tip;              // arrowhead transform

    [Header("Dash (preview only)")]
    public bool  dashed          = false;
    public float dashScrollSpeed = 1.2f;

    LineRenderer lr;
    float        dashOffset;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments + 1;
        lr.useWorldSpace = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (dashed)
        {
            dashOffset += Time.deltaTime * dashScrollSpeed;
            lr.material.SetTextureOffset("_MainTex",
                new Vector2(dashOffset, 0));
        }
    }

    // =========================
    // DRAW
    // Call every frame while active
    // =========================
    public void Draw(Vector3 from, Vector3 to)
    {
        gameObject.SetActive(true);

        // Control point sits above the midpoint
        Vector3 mid     = (from + to) * 0.5f;
        Vector3 control = mid + Vector3.up * arcHeight;

        for (int i = 0; i <= segments; i++)
        {
            float   t   = i / (float)segments;
            Vector3 pos = Bezier(from, control, to, t);
            lr.SetPosition(i, pos);
        }

        // Orient tip to face the curve's final tangent
        if (tip != null)
        {
            Vector3 prev    = Bezier(from, control, to, 0.98f);
            Vector3 dir     = (to - prev).normalized;
            float   angle   = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            tip.position    = to;
            tip.rotation    = Quaternion.Euler(0, 0, angle);
        }
    }

    // Optional — clash arrows stop at a midpoint, not at the target
    public void DrawToMidpoint(Vector3 from, Vector3 to)
    {
        Vector3 midpoint = Vector3.Lerp(from, to, 0.5f);
        Draw(from, midpoint);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (tip != null) tip.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (tip != null) tip.gameObject.SetActive(true);
    }

    // Quadratic Bezier
    static Vector3 Bezier(Vector3 a, Vector3 c, Vector3 b, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * c + t * t * b;
    }
}
