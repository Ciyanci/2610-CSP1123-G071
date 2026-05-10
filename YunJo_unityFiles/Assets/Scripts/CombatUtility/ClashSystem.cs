using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        Debug.Log($"[CLASH] {A.name} vs {B.name}");

        A.HideSpeed();
        B.HideSpeed();

        // =========================
        // POSITIONING
        // =========================

        Vector3 mid = (A.GetClashPosition() + B.GetClashPosition()) / 2f;

        float spacing = 4.5f;

        bool meleeA = A.unitType == UnitType.Melee;
        bool meleeB = B.unitType == UnitType.Melee;

        if (!meleeA && !meleeB) spacing = 6f;
        else spacing = 4.5f;

        Vector3 aPoint = mid + Vector3.left * spacing;
        Vector3 bPoint = mid + Vector3.right * spacing;

        if (meleeA) yield return A.MoveTo(aPoint);
        if (meleeB) yield return B.MoveTo(bPoint);

        yield return new WaitForSeconds(0.2f);

        // =========================
        // CAMERA OPEN
        // =========================

        List<CameraAction> openSeq = new();

        if (meleeA && meleeB)
        {
            openSeq.Add(new CameraAction
            {
                type = CameraActionType.MoveTo,
                position = mid,
                duration = 0.25f
            });

            openSeq.Add(new CameraAction
            {
                type = CameraActionType.Zoom,
                zoom = 4.5f,
                duration = 0.25f
            });
        }
        else
        {
            openSeq.Add(new CameraAction
            {
                type = CameraActionType.Reset
            });
        }

        yield return cam.Play(openSeq);

        // =========================
        // CLASH LOOP
        // =========================

        while (true)
        {
            diceA.Follow(A.headAnchor);
            diceB.Follow(B.headAnchor);

            yield return new WaitForSeconds(0.15f);

            bool doneA = false;
            bool doneB = false;

            DiceData diceAData = a.card.GetDice()[0].data;
            DiceData diceBData = b.card.GetDice()[0].data;

            int rollA = 0;
            int rollB = 0;

            StartCoroutine(diceA.Roll(
                diceAData.minRoll,
                diceAData.maxRoll,
                r => { rollA = r; doneA = true; }
            ));

            StartCoroutine(diceB.Roll(
                diceBData.minRoll,
                diceBData.maxRoll,
                r => { rollB = r; doneB = true; }
            ));

            yield return new WaitUntil(() => doneA && doneB);

            Debug.Log($"[CLASH] Rolls → {A.name}:{rollA} | {B.name}:{rollB}");

            diceA.SetResult(rollA, rollB);
            diceB.SetResult(rollB, rollA);

            yield return new WaitForSeconds(0.25f);

            // =========================
            // TIE
            // =========================
            if (rollA == rollB)
            {
                Debug.Log("[CLASH] Tie → retry");

                A.PlayHit();
                B.PlayHit();

                yield return cam.Play(new List<CameraAction>
                {
                    new CameraAction
                    {
                        type = CameraActionType.Shake,
                        shakeIntensity = 0.2f,
                        duration = 0.2f
                    }
                });

                Vector3 recoilDir = (B.visual.position - A.visual.position).normalized;

                yield return A.Recoil(-recoilDir, 0.4f, 0.12f);
                yield return B.Recoil(recoilDir, 0.4f, 0.12f);

                yield return A.MoveTo(aPoint);
                yield return B.MoveTo(bPoint);

                continue;
            }

            // =========================
            // WIN (NO DAMAGE HERE)
            // =========================

            bool aWins = rollA > rollB;

            CharacterUnit winner = aWins ? A : B;
            CharacterUnit loser = aWins ? B : A;

            Debug.Log($"[CLASH] Winner: {winner.name}");

            winner.PlayAttack();
            loser.PlayHit();

            yield return cam.Play(new List<CameraAction>
            {
                new CameraAction
                {
                    type = CameraActionType.FocusTarget,
                    target = loser.visual,
                    duration = 0.2f
                },
                new CameraAction
                {
                    type = CameraActionType.Zoom,
                    zoom = 3.2f,
                    duration = 0.2f
                }
            });

            Vector3 hitDir = (loser.visual.position - winner.visual.position).normalized;
            yield return loser.Recoil(hitDir, 1.2f, 0.15f);

            // 🔥 IMPORTANT CHANGE:
            // Instead of damage → we only store outcome

            a.isClashWinner = aWins;
            b.isClashWinner = !aWins;

            break;
        }

        // =========================
        // CLEANUP
        // =========================

        yield return new WaitForSeconds(0.3f);

        A.ResetPosition();
        B.ResetPosition();

        A.ShowSpeed();
        B.ShowSpeed();

        diceA.Hide();
        diceB.Hide();

        yield return cam.Play(new List<CameraAction>
        {
            new CameraAction { type = CameraActionType.Reset }
        });

        OnClashFinished?.Invoke();
    }
}