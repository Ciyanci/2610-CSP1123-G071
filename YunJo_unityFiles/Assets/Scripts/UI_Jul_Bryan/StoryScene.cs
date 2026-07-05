using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Data/New Story Scene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    [Header("Chapter Introduction")]
    public bool hasChapterIntro;
    public string chapterNumber;
    public string chapterName;

    [Header("Battle")]
    public bool hasBattleScene;
    public string battleSceneName = "";

    [Header("Battle")]
    public bool loadMap;

    [Header("Scene Data")]
    public List<Sentence> sentences;
    public Sprite backgroud;  //ALL REFERENCES TO "background" must be renamed to "backgroud" whoops
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
        public string jobTitle;         // ← new
        public Sprite characterSprite;
        public CharacterPosition characterPos;
        public Sprite characterSprite2;
        public CharacterPosition characterPos2;
        public Sprite characterSprite3;
        public CharacterPosition characterPos3;
    }
}