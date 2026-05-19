using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;

    void Start()
    {
        bottomBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.backgroud);
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
                        
                    }
                    else
                    {
                        Debug.Log("End of story!");
                    }
                }
                else
                {
                    bottomBar.PlayNextSentence();
                }
            }
        }
    }
}