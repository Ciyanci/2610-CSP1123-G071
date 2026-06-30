using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Data/New Story Scene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    public List<Sentence> sentences;
    public Sprite backgroud;
    public StoryScene nextScene;

    public enum CharacterPosition
    {
        Left, Center, Right, None
    }

    [System.Serializable]
    public struct Sentence
    {
        public string text;
        public Speaker speaker;
        public Sprite characterSprite;
        public CharacterPosition characterPos;
        public Sprite characterSprite2;
        public CharacterPosition characterPos2;
    }
}