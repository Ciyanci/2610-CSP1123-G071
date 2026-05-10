using UnityEngine;
using UnityEngine.UI;

public class DiceIconUI : MonoBehaviour
{
    public Image icon;

    public Sprite slashSprite;
    public Sprite pierceSprite;
    public Sprite bluntSprite;

    public void Setup(DamageType type)
    {
        switch (type)
        {
            case DamageType.Slash:
                icon.sprite = slashSprite;
                break;

            case DamageType.Pierce:
                icon.sprite = pierceSprite;
                break;

            case DamageType.Blunt:
                icon.sprite = bluntSprite;
                break;
        }
    }
}