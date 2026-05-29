using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    [SerializeField] public string speakerName;
    [SerializeField] public Color textColor;
}