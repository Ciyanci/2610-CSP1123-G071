using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusUI : MonoBehaviour
{
    [Header("Owner")]
    public CharacterUnit owner;

    [Header("HP Bar")]
    public Image hpFill;

    [Header("Stagger Bar")]
    public Image staggerFill;

    [Header("Light Bar")]
    public LightBarUI lightBar;

    [Header("Follow — world space bars below character")]
    public Transform followTarget;
    public Vector3 offset = new Vector3(0, -1.2f, 0);

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    public void Bind(CharacterUnit unit)
    {
        owner        = unit;
        followTarget = unit.transform;

        if (lightBar != null)
            lightBar.Bind(unit);

        Refresh();
    }

    void LateUpdate()
    {
        if (followTarget == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(
            followTarget.position + offset
        );

        if (screen.z <= 0) return;

        transform.position = screen;
    }

    public void Refresh()
    {
        if (owner == null) return;

        if (hpFill != null)
            hpFill.fillAmount = Mathf.Clamp01((float)owner.hp / owner.maxHP);

        if (staggerFill != null)
            staggerFill.fillAmount = Mathf.Clamp01((float)owner.stagger / owner.maxStagger);

        lightBar?.Refresh();
    }
}
