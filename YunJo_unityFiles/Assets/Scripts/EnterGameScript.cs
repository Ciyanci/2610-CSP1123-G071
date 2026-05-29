using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterGameScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnterGameClick()
    {
        Debug.Log("Deck Building Screen Selected.");
        SceneManager.LoadScene("DeckBuildingScreen");
    }
}
