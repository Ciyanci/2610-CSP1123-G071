using UnityEngine;
using System.Collections;

public class ClashSystem : MonoBehaviour
{
    public DiceUI dice;

    IEnumerator Buffer(float t)
    {
        yield return new WaitForSeconds(t);
    }

    public IEnumerator Resolve(CombatAction a, CombatAction b)
    {
        CharacterUnit A = a.user;
        CharacterUnit B = b.user;

        yield return A.MoveTo(A.clashAnchor.position);
        yield return B.MoveTo(B.clashAnchor.position);

        yield return Buffer(0.2f);

        int rollA = 0;
        int rollB = 0;

        // sigma loop loop loopeaeaea
        while (true)
        {
            // roll A
            dice.Follow(A.headAnchor);
            yield return dice.Roll(a.card.min, a.card.max, r => rollA = r);

            yield return Buffer(0.2f);

            // roll B
            dice.Follow(B.headAnchor);
            yield return dice.Roll(b.card.min, b.card.max, r => rollB = r);

            yield return Buffer(0.2f);

            // PLEASE SET THE RIGHT COLOUR THIS TIME
            dice.SetResultColor(rollA, rollB);

            yield return Buffer(0.3f);

            if (rollA != rollB) break;

            // tie PLEASE WORK
            dice.text.text = "CLASH!";
            yield return Buffer(0.4f);
        }

        bool aWins = rollA > rollB;

        CharacterUnit winner = aWins ? A : B;
        CharacterUnit loser = aWins ? B : A;

        winner.PlayAttack();
        loser.PlayHit();

        Vector3 dir = (loser.visual.position - winner.visual.position).normalized;

        yield return loser.Recoil(dir, 0.4f, 0.15f);

        // damge = orll
        int dmg = Mathf.RoundToInt(aWins ?
            a.card.damage * (rollA / (float)a.card.max) :
            b.card.damage * (rollB / (float)b.card.max));

        loser.TakeDamage(dmg);

        yield return Buffer(0.3f);
    }
}