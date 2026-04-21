using UnityEngine;
using System.Collections;

public class ClashAnimationPipeline : MonoBehaviour
{
    public CombatCamera cam;

    public IEnumerator PlayClash(CharacterUnit a, CharacterUnit b)
    {
        Vector3 mid = (a.transform.position + b.transform.position) / 2f;

        //zoom camera
        yield return cam.ClashZoom(mid);

        //move into clash lane please
        yield return a.MoveTo(a.clashAnchor.position);
        yield return b.MoveTo(b.clashAnchor.position);

        a.currentState = CharacterUnit.UnitState.Clashing;
        b.currentState = CharacterUnit.UnitState.Clashing;

        //dice rolling initialisationasfndasonf
        int aRoll = 0;
        int bRoll = 0;

        yield return Roll(a, r => aRoll = r);
        yield return Roll(b, r => bRoll = r);

        //clash number logic thingamajajiggy
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
            //tie recoil
            yield return a.Recoil(Vector3.left, 0.2f, 0.1f);
            yield return b.Recoil(Vector3.right, 0.2f, 0.1f);
        }

        yield return cam.Reset();
    }

    IEnumerator Roll(CharacterUnit unit, System.Action<int> result)
    {
        int r = Random.Range(1, 6);
        result?.Invoke(r);
        yield return null;
    }
}