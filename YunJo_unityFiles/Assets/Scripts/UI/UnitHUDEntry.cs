using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUDEntry : MonoBehaviour
{
    [Header("Avatar")]
    public Image avatarImage;

    [Header("Bars")]
    public Image hpFill;
    public Image staggerFill;

    [Header("Name")]
    public TextMeshProUGUI nameText;

    [Header("Speed Bubble — shown at round start")]
    public GameObject      speedBubble;
    public TextMeshProUGUI speedBubbleText;

    CharacterUnit owner;

    public void Bind(CharacterUnit unit)
    {
        owner = unit;
        gameObject.SetActive(true);

        if (nameText    != null) nameText.text    = unit.unitName;
        if (avatarImage != null && unit.idle != null)
            avatarImage.sprite = unit.idle;

        speedBubble?.SetActive(false);
        Refresh();
    }

    public void Refresh()
    {
        if (owner == null) return;
        if (!owner.gameObject.activeInHierarchy)
        {
            if (hpFill      != null) hpFill.fillAmount      = 0f;
            if (staggerFill != null) staggerFill.fillAmount = 0f;
            return;
        }

        if (hpFill != null)
            hpFill.fillAmount = Mathf.Clamp01((float)owner.hp / owner.maxHP);

        if (staggerFill != null)
            staggerFill.fillAmount = Mathf.Clamp01((float)owner.stagger / owner.maxStagger);
    }

    public void ShowSpeedBubble(int speedValue)
    {
        if (speedBubble == null) return;
        speedBubble.SetActive(true);

        if (speedBubbleText != null)
            speedBubbleText.text = speedValue.ToString();

        StartCoroutine(HideBubbleAfter(2.5f));
    }

    System.Collections.IEnumerator HideBubbleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        speedBubble?.SetActive(false);
    }

    public void Unbind()
    {
        owner = null;
        gameObject.SetActive(false);
    }
}
