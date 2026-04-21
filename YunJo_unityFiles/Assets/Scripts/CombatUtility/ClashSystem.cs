using UnityEngine;
using System.Collections;

public class ClashSystem : MonoBehaviour
{
    public DiceUI dice;
    public CombatCamera cam;
    public System.Action OnClashFinished;

    void Awake()
    {
        if (dice == null)
            dice = FindFirstObjectByType<DiceUI>();

        if (cam == null)
            cam = FindFirstObjectByType<CombatCamera>();
    }

    IEnumerator Buffer(float t)
    {
        yield return new WaitForSeconds(t);
    }

    public IEnumerator Resolve(CombatIntent a, CombatIntent b)
    {
        if (a == null || b == null || a.user == null || b.user == null)
            yield break;

        CharacterUnit A = a.user;
        CharacterUnit B = b.user;

        A.SetCombatStartPosition();
        B.SetCombatStartPosition();

        Vector3 mid = (A.GetClashPosition() + B.GetClashPosition()) / 2f;

        Vector3 aPoint = mid + Vector3.left * 3.5f;
        Vector3 bPoint = mid + Vector3.right * 3.5f;

        if (A.unitType == UnitType.Melee)
            yield return A.MoveTo(aPoint);

        if (B.unitType == UnitType.Melee)
            yield return B.MoveTo(bPoint);

        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            int rollA = 0;
            int rollB = 0;

            yield return cam.ClashZoom(A.GetCombatFocusPoint());
            dice.Follow(A.headAnchor);
            yield return dice.Roll(a.card.min, a.card.max, r => rollA = r);

            yield return cam.ClashZoom(B.GetCombatFocusPoint());
            dice.Follow(B.headAnchor);
            yield return dice.Roll(b.card.min, b.card.max, r => rollB = r);

            dice.SetResultColor(rollA, rollB);

            yield return new WaitForSeconds(0.2f);

            if (rollA == rollB)
            {
                A.PlayHit();
                B.PlayHit();

                Vector3 dir = (B.visual.position - A.visual.position).normalized;

                yield return A.Recoil(-dir, 0.3f, 0.12f);
                yield return B.Recoil(dir, 0.3f, 0.12f);

                yield return A.MoveTo(aPoint);
                yield return B.MoveTo(bPoint);

                continue;
            }

            bool aWins = rollA > rollB;

            CharacterUnit winner = aWins ? A : B;
            CharacterUnit loser = aWins ? B : A;

            winner.PlayAttack();
            loser.PlayHit();

            Vector3 hitDir = (loser.visual.position - winner.visual.position).normalized;
            yield return loser.Recoil(hitDir, 0.4f, 0.15f);

            int dmg = Mathf.RoundToInt(
                aWins
                    ? a.card.damage * (rollA / (float)a.card.max)
                    : b.card.damage * (rollB / (float)b.card.max)
            );

            loser.TakeDamage(dmg);
            break;
        }

        A.ResetPosition();
        B.ResetPosition();

        dice.Hide();
        yield return cam.Reset();

        OnClashFinished?.Invoke();
    }
}