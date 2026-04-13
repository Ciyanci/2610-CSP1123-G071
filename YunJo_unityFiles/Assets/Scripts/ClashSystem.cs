using UnityEngine;
using System.Collections;

public class ClashSystem : MonoBehaviour
{
    public DiceUI dice;
    public CombatCamera cam;
    public float clashSpacing = 1.0f;

    IEnumerator Buffer(float t)
    {
        yield return new WaitForSeconds(t);
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    IEnumerator Parallel(params IEnumerator[] routines)
    {
        int running = routines.Length;

        foreach (var r in routines)
            StartCoroutine(Run(r));

        IEnumerator Run(IEnumerator r)
        {
            yield return r;
            running--;
        }

        while (running > 0)
            yield return null;
    }

    public IEnumerator Resolve(CombatAction a, CombatAction b)
    {
        a.resolved = b.resolved = true;

        CharacterUnit A = a.user;
        CharacterUnit B = b.user;

        Vector3 mid = (A.visual.position + B.visual.position) * 0.5f;
        mid.y = Mathf.Min(A.visual.position.y, B.visual.position.y) + 1f;

        Vector3 dir = (B.visual.position - A.visual.position);
        dir.y = 0;
        dir.Normalize();

        float spacing = Mathf.Max(A.HalfWidth, B.HalfWidth) + clashSpacing;

        Vector3 aTarget = mid - dir * spacing;
        Vector3 bTarget = mid + dir * spacing;

        yield return cam.ClashZoom(mid);

        yield return Parallel(
            A.MoveTo(aTarget),
            B.MoveTo(bTarget)
        );

        yield return Buffer(0.1f);

        yield return A.WindUp(0.1f);
        yield return B.WindUp(0.1f);

        yield return Buffer(0.1f);

        int rollA = 0;
        int rollB = 0;

        // ======================
        // ROLL A
        // ======================
        dice.Follow(A.headAnchor);
        yield return dice.DiceTossEffect();
        yield return dice.Roll(a.card.min, a.card.max, r => rollA = r);

        yield return Buffer(0.2f);

        // ======================
        // ROLL B
        // ======================
        dice.Follow(B.headAnchor);
        yield return dice.DiceTossEffect();
        yield return dice.Roll(b.card.min, b.card.max, r => rollB = r);

        yield return Buffer(0.2f);

        // ONLY NOW SET COLOR (FIXED DESYNC)
        dice.SetResultColor(rollA, rollB);

        yield return Buffer(0.2f);

        // tie handling FIX
        if (rollA == rollB)
        {
            yield return Buffer(0.2f);
            yield return Resolve(a, b);
            yield break;
        }

        bool aWins = rollA > rollB;

        CharacterUnit winner = aWins ? A : B;
        CharacterUnit loser = aWins ? B : A;
        CombatAction winAction = aWins ? a : b;
        int winRoll = Mathf.Max(rollA, rollB);

        yield return HitStop(0.08f);

        winner.PlayAttack();
        loser.PlayHit();

        Vector3 recoilDir = (loser.visual.position - winner.visual.position).normalized;

        yield return loser.Recoil(recoilDir, 0.4f, 0.12f);

        yield return cam.ImpactShake(0.12f, 0.12f);

        yield return Buffer(0.15f);

        int dmg = Mathf.RoundToInt(winAction.card.damage * (winRoll / (float)winAction.card.max));
        loser.TakeDamage(dmg);

        yield return cam.Reset();

        dice.text.color = Color.white;

        yield return Buffer(0.25f);
    }
}
