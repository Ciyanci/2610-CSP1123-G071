using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardFrameVisual : MonoBehaviour
{
    [Header("All Images Affected By Rarity")]
    public List<Image> frameImages = new();

    List<Material> runtimeMaterials = new();

    void Awake()
    {
        runtimeMaterials.Clear();

        foreach (var img in frameImages)
        {
            if (img == null)
                continue;
            Material mat = Instantiate(img.material);
            img.material = mat;
            runtimeMaterials.Add(mat);
        }
    }

    public void SetRarity(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                ApplyStyle(
                    0f,
                    Color.white,
                    0f
                );
                break;

            case CardRarity.Uncommon:
                ApplyStyle(
                    0.33f,
                    Color.green,
                    0.2f
                );
                break;

            case CardRarity.Rare:
                ApplyStyle(
                    0.58f,
                    Color.cyan,
                    0.4f
                );
                break;

            case CardRarity.Epic:
                ApplyStyle(
                    0.78f,
                    new Color(0.7f, 0.2f, 1f),
                    0.7f
                );
                break;

            case CardRarity.Legendary:
                ApplyStyle(
                    0.12f,
                    Color.yellow,
                    1.2f
                );
                break;
        }
    }

    void ApplyStyle(
        float hue,
        Color glow,
        float glowStrength)
    {
        foreach (var mat in runtimeMaterials)
        {
            if (mat == null)
                continue;

            mat.SetFloat("_Hue", hue);

            mat.SetColor(
                "_GlowColor",
                glow
            );

            mat.SetFloat(
                "_GlowStrength",
                glowStrength
            );
        }
    }
}
//hi