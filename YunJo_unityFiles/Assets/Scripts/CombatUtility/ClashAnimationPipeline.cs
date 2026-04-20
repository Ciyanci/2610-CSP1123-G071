using UnityEngine;
using System.Collections;

public class ClashAnimationPipeline : MonoBehaviour
{
    public CombatCamera cam;

    public IEnumerator PlayClash(CharacterUnit a, CharacterUnit b)
    {
        Vector3 mid = (a.transform.position + b.transform.position) / 2f;

        // 1. zoom camera
        yield return cam.ClashZoom(mid);

        // 2. move both units into clash lane
        yield return a.MoveTo(a.clashAnchor.position);
        yield return b.MoveTo(b.clashAnchor.position);

        a.currentState = CharacterUnit.UnitState.Clashing;
        b.currentState = CharacterUnit.UnitState.Clashing;

        // 3. dice roll phase
        int aRoll = 0;
        int bRoll = 0;

        yield return Roll(a, r => aRoll = r);
        yield return Roll(b, r => bRoll = r);

        // 4. resolve clash
        if (aRoll > bRoll)
        {
            a.PlayAttack();
            b.PlayHit();
        }
        else if (bRoll > aRoll)
        {
            b.PlayAttack();
            a.PlayHit();
        }
        else
        {
            // tie pushback
            yield return a.Recoil(Vector3.left, 0.2f, 0.1f);
            yield return b.Recoil(Vector3.right, 0.2f, 0.1f);
        }

        // 5. reset camera
        yield return cam.Reset();
    }

    IEnumerator Roll(CharacterUnit unit, System.Action<int> result)
    {
        int r = Random.Range(1, 6);
        result?.Invoke(r);
        yield return null;
    }
}