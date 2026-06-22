using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
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
        operation.allowSceneActivation = true;  
        LoadingScreen.SetActive(false);
    }
}
