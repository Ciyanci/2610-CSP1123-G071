using UnityEngine;

public class ClashLane : MonoBehaviour
{
    public static ClashLane Instance;

    [Header("Lane Positions — set in scene")]
    public Transform centreMark;        //midpoint of the lane
    public Transform leftEngagePoint;   //where left unit stands to attack
    public Transform rightEngagePoint;  //where right unit stands to attack

    [Header("Camera Frame")]
    public Transform cameraFocusPoint;  //camera looks at this

    void Awake() => Instance = this;

    public Vector3 Centre      => centreMark.position;
    public Vector3 LeftEngage  => leftEngagePoint.position;
    public Vector3 RightEngage => rightEngagePoint.position;
    public Vector3 CameraFocus => cameraFocusPoint.position;
}
