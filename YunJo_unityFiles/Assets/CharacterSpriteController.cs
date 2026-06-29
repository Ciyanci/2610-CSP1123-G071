using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteController : MonoBehaviour
{
    public Image leftSlot;
    public Image centerSlot;
    public Image rightSlot;

    public void Show(Sprite sprite, StoryScene.CharacterPosition pos)
    {
        Debug.Log($"[CharacterSprite] Show called — Sprite: {sprite?.name ?? "NULL"}, Position: {pos}");
        
        Hide(); // clear all first
        Image target = pos switch
        {
            StoryScene.CharacterPosition.Left   => leftSlot,
            StoryScene.CharacterPosition.Center => centerSlot,
            StoryScene.CharacterPosition.Right  => rightSlot,
            _ => null
        };

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