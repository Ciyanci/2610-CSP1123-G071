using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialStep
{
    [Header("Tutorial Panel — shown before this step, must be clicked through")]
    public List<string> tutorialPages = new(); //empty = no blocking panel

    [Header("Hint Bar — shown during this step, non-blocking")]
    public string hintText = "";               //empty = no hint shown

    [Header("Gameplay")]
    public bool enableInput      = false;
    public bool keepInputAfter   = false;
    public TutorialWaitCondition waitCondition = TutorialWaitCondition.None;
}
