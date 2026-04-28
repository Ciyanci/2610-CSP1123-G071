using UnityEngine;
using System.Collections;

public class ClashSystem : MonoBehaviour
{
    public DiceUI diceA;
    public DiceUI diceB;

    public CombatCamera cam;
    public System.Action OnClashFinished;

    void Awake()
    {
        if (diceA == null || diceB == null)
        {
            var all = FindObjectsByType<DiceUI>(FindObjectsSortMode.None);
            if (all.Length >= 2)
            {
                diceA = all[0];
                diceB = all[1];
            }
        }

        if (cam == null)
            cam = FindFirstObjectByType<CombatCamera>();
    }

    public IEnumerator Resolve(CombatIntent a, CombatIntent b)
    {
        if (a == null || b == null || a.user == null || b.user == null)
            yield break;

        CharacterUnit A = a.user;
        CharacterUnit B = b.user;

        //hide speed dice
        A.HideSpeed();
        B.HideSpeed();

        // positioning
        Vector3 mid = (A.GetClashPosition() + B.GetClashPosition()) / 2f;

        float meleeDist = 8f;
        float rangedDist = 14f;

        float dist =
            (A.unitType == UnitType.Ranged || B.unitType == UnitType.Ranged)
                ? rangedDist
                : meleeDist;

        Vector3 dir = (B.GetClashPosition() - A.GetClashPosition()).normalized;

        float spacing = 3.5f;

        bool meleeA = A.unitType == UnitType.Melee;
        bool meleeB = B.unitType == UnitType.Melee;

        // melee vs melee
        if (meleeA && meleeB)
        {
            spacing = 4.5f;
        }

        // ranged vs ranged
        else if (!meleeA && !meleeB)
        {
            spacing = 6f;
        }

        // melee vs ranged
        else
        {
            spacing = 4.5f;
        }

        Vector3 aPoint = mid + Vector3.left * spacing;
        Vector3 bPoint = mid + Vector3.right * spacing;

        // melee vs melee
        if (meleeA && meleeB)
        {
            yield return A.MoveTo(aPoint);
            yield return B.MoveTo(bPoint);
        }

        // melee vs ranged
        else if (meleeA && !meleeB)
        {
            yield return A.MoveTo(aPoint);

            // slight reposition
            yield return B.MoveTo(
                B.visual.position + Vector3.right * 0.5f,
                0.12f
            );
        }

        // ranged vs melee
        else if (!meleeA && meleeB)
        {
            yield return B.MoveTo(bPoint);

            yield return A.MoveTo(
                A.visual.position + Vector3.left * 0.5f,
                0.12f
            );
        }

        yield return new WaitForSeconds(0.2f);

        if (meleeA && meleeB)
        {
            yield return cam.ClashCenter(mid);
        }
        else
        {
            yield return cam.Reset();
        }

        while (true)
        {
            int rollA = 0;
            int rollB = 0;

            // 🎲 POSITION DICE
            diceA.Follow(A.headAnchor);
            diceB.Follow(B.headAnchor);

            yield return new WaitForSeconds(0.15f);

            // 🎲 ROLL BOTH (TRUE PARALLEL)
            bool doneA = false;
            bool doneB = false;

            StartCoroutine(diceA.Roll(a.card.min, a.card.max, r =>
            {
                rollA = r;
                doneA = true;
            }));

            StartCoroutine(diceB.Roll(b.card.min, b.card.max, r =>
            {
                rollB = r;
                doneB = true;
            }));

            yield return new WaitUntil(() => doneA && doneB);

            // 🎨 RESULT COLORS
            diceA.SetResult(rollA, rollB);
            diceB.SetResult(rollB, rollA);

            yield return new WaitForSeconds(0.25f);

            // -----------------------------
            // 🤝 TIE → SHAKE
            // -----------------------------
            if (rollA == rollB)
            {
                A.PlayHit();
                B.PlayHit();

                yield return cam.Shake(0.15f, 0.2f);

                Vector3 recoilDir = (B.visual.position - A.visual.position).normalized;

                yield return A.Recoil(-recoilDir, 0.4f, 0.12f);
                yield return B.Recoil(recoilDir, 0.4f, 0.12f);

                yield return A.MoveTo(aPoint);
                yield return B.MoveTo(bPoint);

                continue;
            }

            // -----------------------------
            // 🏆 WIN → FOLLOW LOSER
            // -----------------------------
            bool aWins = rollA > rollB;

            CharacterUnit winner = aWins ? A : B;
            CharacterUnit loser = aWins ? B : A;

            winner.PlayAttack();
            loser.PlayHit();

            // 🔥 SMOOTH FOLLOW (NOT INSTANT SNAP)
            yield return cam.SmoothFollow(loser.visual, 0.25f);

            Vector3 hitDir = (loser.visual.position - winner.visual.position).normalized;

            yield return loser.Recoil(hitDir, 1.2f, 0.15f);

            int dmg = Mathf.RoundToInt(
                aWins
                    ? a.card.damage * (rollA / (float)a.card.max)
                    : b.card.damage * (rollB / (float)b.card.max)
            );

            loser.TakeDamage(dmg);

            break;
        }

        // -----------------------------
        // 🔄 CLEAN RESET
        // -----------------------------
        yield return new WaitForSeconds(0.3f);

        A.ResetPosition();
        B.ResetPosition();

        // ✅ RESTORE SPEED DICE
        A.ShowSpeed();
        B.ShowSpeed();

        diceA.Hide();
        diceB.Hide();

        yield return cam.Reset();

        OnClashFinished?.Invoke();
    }
}