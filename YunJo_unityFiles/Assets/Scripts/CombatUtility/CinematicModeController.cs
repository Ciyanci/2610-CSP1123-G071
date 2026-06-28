using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CinematicModeController : MonoBehaviour
{
    public static CinematicModeController Instance;

    [Header("UI Roots to Hide During Combat")]
    public List<GameObject> planningUIRoots;

    [Header("Camera")]
    public CombatCamera combatCamera;

    [Header("Parallax")]
    public BattlegroundParallax parallax;

    void Awake() => Instance = this;

    public void EnterCinematic()
    {
        foreach (var root in planningUIRoots)
            root?.SetActive(false);
        combatCamera?.SetCinematicView();
        parallax?.EnterCinematic();
    }
    public void ExitCinematic()
    {
        foreach (var root in planningUIRoots)
            root?.SetActive(true);
        combatCamera?.SetPlanningView();
        parallax?.ExitCinematic();
    }
}
