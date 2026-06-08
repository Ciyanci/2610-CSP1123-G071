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
    [Header("Follow")]
    public Vector3 offset = new Vector3(0, -1.2f, 0);
    Camera cam;
    RectTransform rect;
    void Awake()
    {
        cam  = Camera.main;
        rect = GetComponent<RectTransform>();
    }
    public void Bind(CharacterUnit unit)
    {
        owner = unit;
        if (lightBar != null)
            lightBar.Bind(unit);
        Refresh();
    }
    void LateUpdate()
    {
        if (owner == null || owner.visual == null || cam == null) return;
        Vector3 screen = cam.WorldToScreenPoint(
            owner.visual.position + offset);
        if (screen.z <= 0) return;
        rect.position = screen;
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