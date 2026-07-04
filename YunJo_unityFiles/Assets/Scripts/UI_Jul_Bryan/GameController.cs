using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController; // ← renamed field

    void Start()
    {
        bottomBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.backgroud);
        ShowCharacter(currentScene.sentences[0]); // ← show first sentence's character
    }

    void ShowCharacter(StoryScene.Sentence sentence)
    {
        string sprite1Name = sentence.characterSprite != null ? sentence.characterSprite.name : "NULL";
        string sprite2Name = sentence.characterSprite2 != null ? sentence.characterSprite2.name : "NULL";
        Debug.Log($"[GameController] Sentence data — Sprite1: {sprite1Name}, Pos1: {sentence.characterPos}, Sprite2: {sprite2Name}, Pos2: {sentence.characterPos2}");

        characterController.Hide(); // clear both slots first

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
                        bottomBar.PlayScene(currentScene);
                        backgroundController.SwitchImage(currentScene.backgroud);
                        ShowCharacter(currentScene.sentences[0]); // ← show first character of new scene
                    }
                    else
                    {
                        GameManager.Instance.CompleteCurrentScene();
                        Debug.Log("End of story!");
                    }
                }
                else
                {
                    // ← get NEXT index before playing, so character matches upcoming sentence
                    ShowCharacter(currentScene.sentences[bottomBar.GetSentenceIndex() + 1]);
                    bottomBar.PlayNextSentence();
                }
            }
            else
            {
                bottomBar.SkipToEnd(); // ← first click skips typing, second click advances
            }
        }
    }
}