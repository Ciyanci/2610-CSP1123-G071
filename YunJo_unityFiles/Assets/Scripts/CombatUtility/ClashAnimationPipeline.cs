using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClashAnimationPipeline : MonoBehaviour
{
    public CombatCamera cam;

    public IEnumerator PlayClash(CharacterUnit a, CharacterUnit b)
    {
        if (a == null || b == null) yield break;

        Vector3 mid = (a.transform.position + b.transform.position) / 2f;

        // 🎥 CAMERA SETUP
        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction
            {
                type = CameraActionType.MoveTo,
                position = mid,
                duration = 0.25f
            },
            new CameraAction
            {
                type = CameraActionType.Zoom,
                zoom = 4.5f,
                duration = 0.25f
            }
        });

        // MOVE INTO CLASH
        yield return a.MoveTo(a.clashAnchor.position);
        yield return b.MoveTo(b.clashAnchor.position);

        a.currentState = CharacterUnit.UnitState.Clashing;
        b.currentState = CharacterUnit.UnitState.Clashing;

        int aRoll = 0;
        int bRoll = 0;

        yield return Roll(a, r => aRoll = r);
        yield return Roll(b, r => bRoll = r);

        Debug.Log($"[PIPELINE] Rolls → {aRoll} vs {bRoll}");

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
            yield return cam.Play(new List<CameraAction>
            {
                new CameraAction
                {
                    type = CameraActionType.Shake,
                    shakeIntensity = 0.2f,
                    duration = 0.2f
                }
            });

            yield return a.Recoil(Vector3.left, 0.2f, 0.1f);
            yield return b.Recoil(Vector3.right, 0.2f, 0.1f);
        }

        // 🎥 RESET
        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction { type = CameraActionType.Reset }
        });
    }

    IEnumerator Roll(CharacterUnit unit, System.Action<int> result)
    {
        int r = Random.Range(1, 6);
        result?.Invoke(r);
        yield return new WaitForSeconds(0.2f);
    }
}