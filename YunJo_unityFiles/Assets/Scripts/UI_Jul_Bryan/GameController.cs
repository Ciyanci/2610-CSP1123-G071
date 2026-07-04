using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController;
    public ChapterIntroController chapterIntro; // ← new

    void Start()
    {
        PlayCurrentScene();
    }

    void PlayCurrentScene()
    {
        if (currentScene.hasChapterIntro)
        {
            chapterIntro.Show(currentScene.chapterNumber, currentScene.chapterName, () =>
            {
                bottomBar.PlayScene(currentScene);
                backgroundController.SetImage(currentScene.backgroud);
                ShowCharacter(currentScene.sentences[0]);
            });
        }
        else
        {
            bottomBar.PlayScene(currentScene);
            backgroundController.SetImage(currentScene.backgroud);
            ShowCharacter(currentScene.sentences[0]);
        }
    }

    void ShowCharacter(StoryScene.Sentence sentence)
    {
        string sprite1Name = sentence.characterSprite != null ? sentence.characterSprite.name : "NULL";
        string sprite2Name = sentence.characterSprite2 != null ? sentence.characterSprite2.name : "NULL";
        Debug.Log($"[GameController] Sentence data — Sprite1: {sprite1Name}, Pos1: {sentence.characterPos}, Sprite2: {sprite2Name}, Pos2: {sentence.characterPos2}");

        characterController.Hide();

        if (sentence.characterPos != StoryScene.CharacterPosition.None && sentence.characterSprite != null)
        {
            characterController.Show(sentence.characterSprite, sentence.characterPos);
        }

        if (sentence.characterPos2 != StoryScene.CharacterPosition.None && sentence.characterSprite2 != null)
        {
            characterController.Show(sentence.characterSprite2, sentence.characterPos2);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (bottomBar.IsCompleted())
            {
                if (bottomBar.IsLastSentence())
                {
                    if (currentScene.nextScene != null)
                    {
                        currentScene = currentScene.nextScene;
                        backgroundController.SwitchImage(currentScene.backgroud);
                        PlayCurrentScene(); // ← replaces the 3 manual lines
                    }
                    else
                    {
                        GameManager.Instance.CompleteCurrentScene();
                        Debug.Log("End of story!");
                    }
                }
                else
                {
                    ShowCharacter(currentScene.sentences[bottomBar.GetSentenceIndex() + 1]);
                    bottomBar.PlayNextSentence();
                }
            }
            else
            {
                bottomBar.SkipToEnd();
            }
        }
    }
}