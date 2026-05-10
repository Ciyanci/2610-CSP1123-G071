using UnityEngine;
using UnityEngine.UI;

public class CardFrameVisual : MonoBehaviour
{
    public Image frameImage;

    Material runtimeMat;

    void Awake()
    {
        runtimeMat =
            Instantiate(frameImage.material);

        frameImage.material = runtimeMat;
    }

    public void SetRarity(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                SetHue(0f);
                SetGlow(Color.white, 0f);
                break;

            case CardRarity.Uncommon:
                SetHue(0.33f); // green
                SetGlow(Color.green, 0.2f);
                break;

            case CardRarity.Rare:
                SetHue(0.58f); // blue
                SetGlow(Color.cyan, 0.4f);
                break;

            case CardRarity.Epic:
                SetHue(0.78f); // purple
                SetGlow(new Color(0.7f, 0.2f, 1f), 0.7f);
                break;

            case CardRarity.Legendary:
                SetHue(0.12f); // gold
                SetGlow(Color.yellow, 1.2f);
                break;
        }
    }

    void SetHue(float value)
    {
        runtimeMat.SetFloat("_Hue", value);
    }

    void SetGlow(Color c, float strength)
    {
        runtimeMat.SetColor("_GlowColor", c);
        runtimeMat.SetFloat("_GlowStrength", strength);
    }
}