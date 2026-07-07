using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteController : MonoBehaviour
{
    public Image leftSlot;
    public Image leftSlot2;
    public Image centerSlot;
    public Image centerSlot2;
    public Image rightSlot;
    public Image rightSlot2;
    public Image rightSlot3;
    public Image centerSlot3;
    public Image rightSlot4;
    public Image centerSlot4;

    public void Show(Sprite sprite, StoryScene.CharacterPosition pos)
    {
        Debug.Log($"[CharacterSprite] Show called — Sprite: {sprite?.name ?? "NULL"}, Position: {pos}");

        Image target = GetSlot(pos);

        if (target != null)
        {
            target.sprite = sprite;
            SetAlpha(target, 1f);
            Debug.Log($"[CharacterSprite] Showing sprite '{sprite?.name}' in {pos} slot");
        }
        else
        {
            Debug.LogWarning($"[CharacterSprite] No slot found for position: {pos}");
        }
    }

    public void Hide()
    {
        Debug.Log("[CharacterSprite] Hide called — clearing all slots");
        SetAlpha(leftSlot, 0f);
        SetAlpha(centerSlot, 0f);
        SetAlpha(rightSlot, 0f);
        SetAlpha(rightSlot2, 0f);
        SetAlpha(leftSlot2, 0f);
        SetAlpha(centerSlot2, 0f);
        SetAlpha(rightSlot3, 0f);
        SetAlpha(rightSlot4, 0f);
        SetAlpha(centerSlot3, 0f);
        SetAlpha(centerSlot4, 0f);
    }

    private Image GetSlot(StoryScene.CharacterPosition pos)
    {
        return pos switch
        {
            StoryScene.CharacterPosition.Left    => leftSlot,
            StoryScene.CharacterPosition.Left2   => leftSlot2,
            StoryScene.CharacterPosition.Center  => centerSlot,
            StoryScene.CharacterPosition.Center2 => centerSlot2,
            StoryScene.CharacterPosition.Right   => rightSlot,
            StoryScene.CharacterPosition.Right2  => rightSlot2, // ← new
            StoryScene.CharacterPosition.Right3  => rightSlot3,
            StoryScene.CharacterPosition.Right4  => rightSlot4,
            StoryScene.CharacterPosition.Center3  => centerSlot3,
            StoryScene.CharacterPosition.Center4 => centerSlot4,
            _ => null
        };
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            Debug.LogError($"[CharacterSprite] SetAlpha failed — Image slot is not assigned in Inspector!");
            return;
        }
        Color c = image.color;
        c.a = alpha;
        image.color = c;
        Debug.Log($"[CharacterSprite] Set alpha {alpha} on {image.gameObject.name}");
    }
}