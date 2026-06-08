using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CombatDiceElement : MonoBehaviour
{
    [Header("Refs")]
    public Image background;
    public TextMeshProUGUI valueText;
    public Image breakOverlay;

    [Header("Behaviour Colors")]
    public Color attackColor = new Color(0.75f, 0.15f, 0.15f, 1f);
    public Color defendColor = new Color(0.15f, 0.35f, 0.75f, 1f);
    public Color evadeColor  = new Color(0.15f, 0.65f, 0.35f, 1f);
    public Color buffColor   = new Color(0.65f, 0.55f, 0.15f, 1f);

    bool broken = false;

    //called by CombatDiceGroupUI.RefreshActiveDie each time the die advances
    public void Setup(DiceData data)
    {
        StopAllCoroutines();
        broken = false;
        gameObject.SetActive(true);
        breakOverlay?.gameObject.SetActive(false);
        valueText.text  = "?";
        valueText.color = Color.white;
        valueText.gameObject.SetActive(true);
        if (background != null)
            background.color = data.effect switch
            {
                DiceBehaviour.Attack => attackColor,
                DiceBehaviour.Defend => defendColor,
                DiceBehaviour.Evade  => evadeColor,
                DiceBehaviour.Buff   => buffColor,
                _                    => attackColor
            };
    }

    public void SetValue(int value)
    {
        if (broken) return;
        valueText.text = value.ToString();
    }

    public void SetResult(bool won)
    {
        if (broken) return;
        valueText.color = won ? Color.green : Color.red;
    }

    public void Break()
    {
        if (broken) return;
        broken = true;
        StartCoroutine(BreakAnim());
    }

    IEnumerator BreakAnim()
    {
        valueText.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        float t = 0f;
        while (t < 0.3f)
        {
            float a = Mathf.Lerp(1f, 0f, t / 0.3f);
            valueText.color = new Color(1f, 0f, 0f, a);
            t += Time.deltaTime;
            yield return null;
        }

        valueText.gameObject.SetActive(false);
        breakOverlay?.gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}