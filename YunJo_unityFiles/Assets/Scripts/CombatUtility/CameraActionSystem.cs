using UnityEngine;
using System.Collections.Generic;

public enum CameraActionType
{
    MoveTo,
    FocusTarget,
    Zoom,
    Shake,
    Reset,
    FrameTargets
}

[System.Serializable]
public class CameraAction
{
    public CameraActionType type;

    public Vector3 position;
    public Transform target;
    public List<Transform> targets;

    public float zoom;
    public float duration = 0.3f;

    public float shakeIntensity;
}