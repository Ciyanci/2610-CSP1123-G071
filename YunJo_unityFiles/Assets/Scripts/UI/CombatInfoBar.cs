using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatInfoBar : MonoBehaviour
{
    public static CombatInfoBar Instance;

    [Header("Left Panel — player unit")]
    public TextMeshProUGUI leftUnitName;
    public Image leftHpFill;
    public TextMeshProUGUI leftHpText;
    public Image leftStaggerFill;
    public TextMeshProUGUI leftStaggerText;

    [Header("Right Panel — enemy unit")]
    public TextMeshProUGUI rightUnitName;
    public Image rightHpFill;
    public TextMeshProUGUI rightHpText;
    public Image rightStaggerFill;
    public TextMeshProUGUI rightStaggerText;

    [Header("Centre Panel")]
    public GameObject centrePanel;
    public TextMeshProUGUI centreLabel;
    public Transform leftCardRoot;
    public Transform rightCardRoot;

    //pre-placed, not instantiated prefabs ok
    [Header("Card Previews")]
    public InfoBarCardView leftCardView;
    public InfoBarCardView rightCardView;

    [Header("Odds")]
    public TextMeshProUGUI oddsLabel;

    [Header("Colors")]
    public Color clashColor = new Color(0.85f, 0.2f,  0.2f,  1f);
    public Color unopposedColor = new Color(0.2f,  0.75f, 0.2f,  1f);

    void Awake()
    {
        Instance = this;
        centrePanel?.SetActive(false);
    }

    //default (player on left)
    public void ShowDefault()
    {
        centrePanel?.SetActive(false);
        ClearPanel(rightUnitName, rightHpFill, rightHpText,
                   rightStaggerFill, rightStaggerText);

        rightCardView?.Clear();
        var players = UnitRegistry.Instance?.players;
        if (players != null && players.Count > 0)
            BindLeft(players[0]);
    }

    //show unit
    public void ShowUnit(CharacterUnit unit)
    {
        if (unit == null) return;
        centrePanel?.SetActive(false);
        bool isPlayer = UnitRegistry.Instance.players.Contains(unit);
        if (isPlayer) BindLeft(unit);
        else BindRight(unit);
    }

    //slot info
    public void ShowSlotInfo(SpeedSlot slot)
    {
        if (slot == null) return;
        CharacterUnit owner = slot.owner;
        bool ownerIsPlayer  = UnitRegistry.Instance.players.Contains(owner);
        if (ownerIsPlayer) BindLeft(owner);
        else               BindRight(owner);
        CharacterUnit target = slot.target;
        if (target != null)
        {
            if (ownerIsPlayer) BindRight(target);
            else               BindLeft(target);

            ShowIntentInfo(slot, owner, target, ownerIsPlayer);
        }
        else
        {
            centrePanel?.SetActive(false);
        }
    }

    //intent info
    void ShowIntentInfo(
        SpeedSlot slot,
        CharacterUnit owner,
        CharacterUnit target,
        bool ownerIsPlayer)
    {
        centrePanel?.SetActive(true);

        SpeedSlot counterSlot = FindCounterSlot(slot);
        bool isClash = counterSlot != null;

        if (centreLabel != null)
        {
            centreLabel.text = isClash ? "CLASHING" : "UNOPPOSED";
            centreLabel.color = isClash ? clashColor : unopposedColor;
        }

        SpeedSlot playerSlot = ownerIsPlayer ? slot : counterSlot;
        SpeedSlot enemySlot  = ownerIsPlayer ? counterSlot : slot;

        //use infobarcardview directly (no prefabs, gotta make this everytime)
        leftCardView?.Setup(playerSlot?.assignedCard);
        rightCardView?.Setup(enemySlot?.assignedCard);

        if (oddsLabel != null)
        {
            if (isClash &&
                playerSlot?.assignedCard != null &&
                enemySlot?.assignedCard != null)
            {
                var dieP = playerSlot.assignedCard.GetDiceSafe(0);
                var dieE = enemySlot.assignedCard.GetDiceSafe(0);
                if (dieP != null && dieE != null)
                {
                    float odds = ClashOddsCalculator.WinProbability(
                        dieP.minRoll, dieP.maxRoll, dieP.power,
                        dieE.minRoll, dieE.maxRoll, dieE.power);

                    oddsLabel.text  = ClashOddsCalculator.OddsLabel(odds);
                    oddsLabel.color = ClashOddsCalculator.OddsColor(odds);
                    oddsLabel.gameObject.SetActive(true);
                }
                else oddsLabel.gameObject.SetActive(false);
            }
            else oddsLabel.gameObject.SetActive(false);
        }
    }

    //clear
    public void Clear()
    {
        centrePanel?.SetActive(false);
        leftCardView?.Clear();
        rightCardView?.Clear();
        ClearPanel(leftUnitName,  leftHpFill,  leftHpText,
                   leftStaggerFill,  leftStaggerText);
        ClearPanel(rightUnitName, rightHpFill, rightHpText,
                   rightStaggerFill, rightStaggerText);
    }

    //helpers
    void BindLeft(CharacterUnit unit)
    {
        if (unit == null) return;
        if (leftUnitName != null) leftUnitName.text = unit.unitName;
        if (leftHpFill != null) leftHpFill.fillAmount = Mathf.Clamp01((float)unit.hp / unit.maxHP);
        if (leftHpText != null) leftHpText.text = $"{unit.hp} / {unit.maxHP}";
        if (leftStaggerFill != null) leftStaggerFill.fillAmount = Mathf.Clamp01((float)unit.stagger / unit.maxStagger);
        if (leftStaggerText != null) leftStaggerText.text = $"{unit.stagger} / {unit.maxStagger}";
    }

    void BindRight(CharacterUnit unit)
    {
        if (unit == null) return;
        if (rightUnitName != null) rightUnitName.text = unit.unitName;
        if (rightHpFill != null) rightHpFill.fillAmount = Mathf.Clamp01((float)unit.hp / unit.maxHP);
        if (rightHpText != null) rightHpText.text = $"{unit.hp} / {unit.maxHP}";
        if (rightStaggerFill != null) rightStaggerFill.fillAmount = Mathf.Clamp01((float)unit.stagger / unit.maxStagger);
        if (rightStaggerText != null) rightStaggerText.text = $"{unit.stagger} / {unit.maxStagger}";
    }

    void ClearPanel(
        TextMeshProUGUI name,
        Image hpFill,       TextMeshProUGUI hpText,
        Image staggerFill,  TextMeshProUGUI staggerText)
    {
        if (name != null) name.text = string.Empty;
        if (hpFill != null) hpFill.fillAmount = 0;
        if (hpText != null) hpText.text = string.Empty;
        if (staggerFill != null) staggerFill.fillAmount = 0;
        if (staggerText != null) staggerText.text = string.Empty;
    }

    SpeedSlot FindCounterSlot(SpeedSlot slot)
    {
        if (slot?.target == null) return null;

        foreach (var s in slot.target.speedSlots)
        {
            if (s.target == slot.owner &&
                (s.state == SlotState.Planned ||
                 s.state == SlotState.Committed))
                return s;
        }
        return null;
    }
}