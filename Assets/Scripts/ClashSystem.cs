using UnityEngine;
using System.Collections;

public class ClashSystem : MonoBehaviour
{
    public DiceUI dice;
    public CombatCamera cam;
    public float clashSpacing = 1.2f;
    public CharacterUnit sr;

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

    IEnumerator SlowMo(float targetScale, float duration)
    {
        float start = Time.timeScale;
        float t = 0;

        while (t < duration)
        {
            Time.timeScale = Mathf.Lerp(start, targetScale, t / duration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = targetScale;
    }

    public IEnumerator Resolve(CombatAction a, CombatAction b)
    {
        a.resolved = b.resolved = true;

        CharacterUnit A = a.user;
        CharacterUnit B = b.user;
        Vector3 Apos = A.visual.position;
        Vector3 Bpos = B.visual.position;

        Vector3 mid = (Apos + Bpos) * 0.5f;
        mid.y = 0;
        mid.z = Apos.z;

        Vector3 dir = (Bpos - Apos);
        dir.y = 0;
        dir.Normalize();

        float spacing = 0.8f;
        Vector3 aTarget = mid - dir * spacing;
        Vector3 bTarget = mid + dir * spacing;

        yield return cam.ClashZoom(mid);

        yield return Parallel(
            A.MoveTo(aTarget),
            B.MoveTo(bTarget)
        );

        yield return A.WindUp(0.15f);
        yield return B.WindUp(0.15f);

        int rollA = 0;
        int rollB = 0;

        yield return SlowMo(0.35f, 0.1f);

        dice.Follow(A.headAnchor);
        yield return dice.Roll(a.card.min, a.card.max,r=> rollA = r);
        yield return cam.ImpactBurst();

        yield return new WaitForSeconds(0.15f);

        dice.Follow(B.headAnchor);
        yield return dice.Roll(b.card.min, b.card.max,r=> rollB = r);
        yield return cam.ImpactBurst();

        dice.SetResultColor(rollA,rollB);

        yield return SlowMo(1f, 0.15f);

        CharacterUnit winner = rollA >= rollB ? A : B;
        CharacterUnit loser = rollA >= rollB ? B : A;
        CombatAction winAction = rollA >= rollB ? a : b;
        int winRoll = Mathf.Max(rollA, rollB);
        yield return cam.LeanToward(winner.visual.position);

        winner.PlayAttack();
        loser.PlayHit();

        yield return new WaitForSeconds(0.25f);

        int dmg = Mathf.RoundToInt(winAction.card.damage * (winRoll / (float)winAction.card.max));

        loser.TakeDamage(dmg);

        yield return cam.Reset();

        dice.text.color = Color.white;
    }
}
