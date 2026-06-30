using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{
    [Header("Loading Screen UI")]
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    public void NewGame(int sceneId)
    {
        GameManager.Instance.DeleteSave();

        GameManager.Instance.SaveGame();

        StartCoroutine(LoadSceneAsync(sceneId)); //begin loading the next scene
    }

    public void ContinuGame(int sceneId)
    {
        GameManager.Instance.LoadGame();

        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void LoadScene(int sceneId)
    {
        GameManager.Instance.SaveGame();
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId) //ienumerator cuz it allows loading screen and bar to update while it loads the next scene, makes it so that it loads over multiple frames
    {
        LoadingScreen.SetActive(true);
        LoadingBarFill.fillAmount = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;

        float timer = 0f;
        float minLoadTime = 2.5f;


        while (timer < minLoadTime || operation.progress < 0.9f)
        {
            timer += Time.unscaledDeltaTime;
            float loadProgress = Mathf.Clamp01(operation.progress/0.9f);
            float timerProgress = Mathf.Clamp01(timer/minLoadTime);
            LoadingBarFill.fillAmount = Mathf.Min(loadProgress, timerProgress);
            yield return null;
        }
        LoadingBarFill.fillAmount = 1f;
        operation.allowSceneActivation = true;  
        LoadingScreen.SetActive(false);
    }
}
