using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController;

    void Start()
    {
        Debug.Log("[GameController] Start called");
        Debug.Log($"[GameController] currentScene: {(currentScene == null ? "NULL" : currentScene.name)}");
        Debug.Log($"[GameController] bottomBar: {(bottomBar == null ? "NULL" : "OK")}");
        Debug.Log($"[GameController] backgroundController: {(backgroundController == null ? "NULL" : "OK")}");
        Debug.Log($"[GameController] characterController: {(characterController == null ? "NULL" : "OK")}");

        bottomBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.backgroud);
        ShowCharacter(currentScene.sentences[0]);
    }

    void ShowCharacter(StoryScene.Sentence sentence)
    {
        Debug.Log($"[GameController] ShowCharacter called — sprite: {(sentence.characterSprite == null ? "NULL" : sentence.characterSprite.name)}, pos: {sentence.characterPos}");

        if (characterController == null)
        {
            Debug.LogError("[GameController] characterController is NULL — assign it in the Inspector!");
            return;
        }

        if (sentence.characterPos == StoryScene.CharacterPosition.None || sentence.characterSprite == null)
        {
            Debug.Log("[GameController] No character to show, calling Hide");
            characterController.Hide();
        }
        else
        {
            Debug.Log("[GameController] Calling Show on characterController");
            characterController.Show(sentence.characterSprite, sentence.characterPos);
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
                        ShowCharacter(currentScene.sentences[0]);
                    }
                    else
                    {
                        Debug.Log("[GameController] End of story!");
                    }
                }
                else
                {
                    ShowCharacter(currentScene.sentences[bottomBar.GetSentenceIndex() + 1]);
                    bottomBar.PlayNextSentence();
                }
            }
        }
    }
}