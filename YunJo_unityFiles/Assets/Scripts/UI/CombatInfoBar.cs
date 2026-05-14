using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatInfoBar : MonoBehaviour
{
    public static CombatInfoBar Instance;

    // =========================
    // LEFT PANEL — player unit
    // =========================
    [Header("Left Panel")]
    public TextMeshProUGUI leftUnitName;
    public Image           leftHpFill;
    public TextMeshProUGUI leftHpText;
    public Image           leftStaggerFill;
    public TextMeshProUGUI leftStaggerText;

    // =========================
    // RIGHT PANEL — enemy unit
    // =========================
    [Header("Right Panel")]
    public TextMeshProUGUI rightUnitName;
    public Image           rightHpFill;
    public TextMeshProUGUI rightHpText;
    public Image           rightStaggerFill;
    public TextMeshProUGUI rightStaggerText;

    // =========================
    // CENTRE — intent / clash info
    // =========================
    [Header("Centre Panel")]
    public GameObject      centrePanel;
    public TextMeshProUGUI centreLabel;     // "⚔ CLASHING" or "✓ UNOPPOSED"
    public Transform leftCardRoot;
    public Transform rightCardRoot;

    public CardView cardPreviewPrefab;

    CardView leftPreview;
    CardView rightPreview;

    [Header("Odds")]
    public TextMeshProUGUI oddsLabel;   // e.g. "Favoured to Win"


    [Header("Colors")]
    public Color clashColor     = new Color(0.85f, 0.2f,  0.2f,  1f);
    public Color unopposedColor = new Color(0.2f,  0.75f, 0.2f,  1f);
    public Color neutralColor   = Color.white;

    void Awake()
    {
        Instance = this;
        centrePanel?.SetActive(false);
    }

    // =========================
    // SHOW UNIT (clicking a unit directly)
    // =========================
    public void ShowUnit(CharacterUnit unit)
    {
        if (unit == null) return;

        centrePanel?.SetActive(false);

        bool isPlayer = UnitRegistry.Instance.players.Contains(unit);

        if (isPlayer)
            BindLeft(unit);
        else
            BindRight(unit);
    }

    // =========================
    // SHOW SLOT INFO (clicking a speed slot)
    // Shows both units involved and clash/unopposed state in centre
    // =========================
    public void ShowSlotInfo(SpeedSlot slot)
    {
        if (slot == null) return;

        CharacterUnit owner = slot.owner;

        // Always show the slot owner on their side
        bool ownerIsPlayer = UnitRegistry.Instance.players.Contains(owner);

        if (ownerIsPlayer)
            BindLeft(owner);
        else
            BindRight(owner);

        // If the slot has a target, show them on the other side
        CharacterUnit target = slot.target;

        if (target != null)
        {
            if (ownerIsPlayer)
                BindRight(target);
            else
                BindLeft(target);

            // Centre intent panel
            ShowIntentInfo(slot, owner, target, ownerIsPlayer);
        }
        else
        {
            centrePanel?.SetActive(false);
        }
    }

    // =========================
    // CENTRE INTENT INFO
    // =========================
    void ShowIntentInfo(
        SpeedSlot slot,
        CharacterUnit owner,
        CharacterUnit target,
        bool ownerIsPlayer)
    {
        centrePanel?.SetActive(true);

        SpeedSlot counterSlot = FindCounterSlot(slot);
        bool isClash          = counterSlot != null;

        if (centreLabel != null)
        {
            centreLabel.text  = isClash ? "⚔  CLASHING" : "✓  UNOPPOSED";
            centreLabel.color = isClash ? clashColor : unopposedColor;
        }

        SpeedSlot playerSlot = ownerIsPlayer ? slot        : counterSlot;
        SpeedSlot enemySlot  = ownerIsPlayer ? counterSlot : slot;

        ShowCardPreview(
            playerSlot?.assignedCard,
            enemySlot?.assignedCard
        );

        // Odds — only meaningful during a clash
        if (oddsLabel != null)
        {
            if (isClash &&
                playerSlot?.assignedCard != null &&
                enemySlot?.assignedCard  != null)
            {
                // Use the first die of each card for the probability estimate
                var dieP = playerSlot.assignedCard.GetDiceSafe(0);
                var dieE = enemySlot.assignedCard.GetDiceSafe(0);

                if (dieP != null && dieE != null)
                {
                    float odds = ClashOddsCalculator.WinProbability(
                        dieP.minRoll, dieP.maxRoll, dieP.power,
                        dieE.minRoll, dieE.maxRoll, dieE.power
                    );

                    oddsLabel.text  = ClashOddsCalculator.OddsLabel(odds);
                    oddsLabel.color = ClashOddsCalculator.OddsColor(odds);
                    oddsLabel.gameObject.SetActive(true);
                }
                else
                {
                    oddsLabel.gameObject.SetActive(false);
                }
            }
            else
            {
                oddsLabel.gameObject.SetActive(false);
            }
        }
    }

    void ShowCardPreview(Card left, Card right)
    {
        if (leftPreview != null)
            Destroy(leftPreview.gameObject);

        if (rightPreview != null)
            Destroy(rightPreview.gameObject);

        if (left != null)
        {
            leftPreview = Instantiate(cardPreviewPrefab, leftCardRoot);
            leftPreview.Setup(left, null);
        }

        if (right != null)
        {
            rightPreview = Instantiate(cardPreviewPrefab, rightCardRoot);
            rightPreview.Setup(right, null);
        }
    }


    // =========================
    // CLEAR
    // =========================
    public void Clear()
    {
        centrePanel?.SetActive(false);

        ClearPanel(leftUnitName,  leftHpFill,  leftHpText,
                   leftStaggerFill,  leftStaggerText);
        ClearPanel(rightUnitName, rightHpFill, rightHpText,
                   rightStaggerFill, rightStaggerText);
    }

    // =========================
    // HELPERS
    // =========================
    void BindLeft(CharacterUnit unit)
    {
        if (unit == null) return;

        if (leftUnitName    != null) leftUnitName.text    = unit.unitName;
        if (leftHpFill      != null) leftHpFill.fillAmount =
            Mathf.Clamp01((float)unit.hp / unit.maxHP);
        if (leftHpText      != null) leftHpText.text      =
            $"{unit.hp} / {unit.maxHP}";
        if (leftStaggerFill != null) leftStaggerFill.fillAmount =
            Mathf.Clamp01((float)unit.stagger / unit.maxStagger);
        if (leftStaggerText != null) leftStaggerText.text =
            $"{unit.stagger} / {unit.maxStagger}";
    }

    void BindRight(CharacterUnit unit)
    {
        if (unit == null) return;

        if (rightUnitName    != null) rightUnitName.text    = unit.unitName;
        if (rightHpFill      != null) rightHpFill.fillAmount =
            Mathf.Clamp01((float)unit.hp / unit.maxHP);
        if (rightHpText      != null) rightHpText.text      =
            $"{unit.hp} / {unit.maxHP}";
        if (rightStaggerFill != null) rightStaggerFill.fillAmount =
            Mathf.Clamp01((float)unit.stagger / unit.maxStagger);
        if (rightStaggerText != null) rightStaggerText.text =
            $"{unit.stagger} / {unit.maxStagger}";
    }

    void ClearPanel(
        TextMeshProUGUI name,
        Image hpFill, TextMeshProUGUI hpText,
        Image staggerFill, TextMeshProUGUI staggerText)
    {
        if (name        != null) name.text               = string.Empty;
        if (hpFill      != null) hpFill.fillAmount        = 0;
        if (hpText      != null) hpText.text              = string.Empty;
        if (staggerFill != null) staggerFill.fillAmount   = 0;
        if (staggerText != null) staggerText.text         = string.Empty;
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

    string FormatCardInfo(Card card)
    {
        if (card == null) return string.Empty;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(card.Name);

        foreach (var die in card.GetDice())
            sb.AppendLine($"  {die.effect} {die.damageType} {die.minRoll}-{die.maxRoll}+{die.power}");

        return sb.ToString().TrimEnd();
    }
}
