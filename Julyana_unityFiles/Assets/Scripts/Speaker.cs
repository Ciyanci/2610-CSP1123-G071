using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
[System.Serializable]

public class Speaker : ScriptableObject
{
    public string SpeakerName;
    public Color textColor;
}