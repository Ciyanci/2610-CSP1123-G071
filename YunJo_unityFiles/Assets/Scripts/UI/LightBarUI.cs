using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LightBarUI : MonoBehaviour
{
    [Header("Owner")]
    public CharacterUnit owner;

    [Header("Slots")]
    //assign these in order in the Inspector (one Image per max light slot)
    public List<Image> lightSlots = new();

    [Header("Sprites")]
    public Sprite filledSprite;
    public Sprite emptySprite;

    [Header("Follow")]
    public Transform followTarget;

    public Vector3 offset = new Vector3(0, -1.6f, 0);

    Camera cam;

    public void Bind(CharacterUnit unit)
    {
        owner = unit;
        followTarget = unit.headAnchor;
        Refresh();
    }

    public void Refresh()
    {
        if (owner == null) return;

        for (int i = 0; i < lightSlots.Count; i++)
        {
            if (lightSlots[i] == null) continue;

            bool filled = i < owner.currentLight;
            lightSlots[i].sprite = filled ? filledSprite : emptySprite;
        }
    }

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (owner == null || owner.visual == null || cam == null) return;
        //follows visual directly with offset
        Vector3 worldPos = owner.visual.position + offset;
        Vector3 screen   = cam.WorldToScreenPoint(worldPos);
        if (screen.z <= 0) return;
        transform.position = screen;
    }
}
