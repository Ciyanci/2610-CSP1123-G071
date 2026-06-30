using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Drag every CardData asset here")]
    public List<CardData> allCardData = new();
    [Header("Gameplay/Visual Novel scenes go here")]
    public List<string> gameplayScenes = new();
    public List<string> completedSceneNames = new();


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadGame();
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockCard(CardData card) //unlocks a new card and immediately saves the game
    {
        CardInventory.Add(card);
        SaveGame();
    }

    public void CompleteCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (!gameplayScenes.Contains(sceneName))
        {
            Debug.Log($"[STAGE] {sceneName} is not a gameplay/visual novel stage ");
            return;
        }

        if (!completedSceneNames.Contains(sceneName))
        {
            completedSceneNames.Add(sceneName);
            SaveGame();

            Debug.Log($"[STAGE] Completed scene: {sceneName}");
        }
    }

    public bool IsSceneCompleted(string sceneName)
    {
        return completedSceneNames.Contains(sceneName);
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        foreach (CardData card in CardInventory.GetAll()) //convert CardData objects into card names
        {
            data.unlockedCardNames.Add(card.Name);
        }

        data.completedSceneNames = new List<string>(completedSceneNames);

        SaveSystem.Save(data);

        Debug.Log($"[SAVE] Saved {data.unlockedCardNames.Count} cards");
        Debug.Log($"[SAVE] Saved {data.completedSceneNames.Count} completed scenes.");
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();

        CardInventory.Clear(); 

        foreach (string name in data.unlockedCardNames) //convert saved card names back into CardData objects
        {
            CardData found = allCardData.Find(card => card.Name == name);

            if (found != null)
            {
                CardInventory.Add(found);
            }
            else
            {
                Debug.LogWarning($"[LOAD] Card not found; {name}");
            }
        }

        completedSceneNames = new List<string>(data.completedSceneNames);

        Debug.Log($"[LOAD] Loaded {CardInventory.GetAll().Count} cards");
        Debug.Log($"[LOAD] Loaded {completedSceneNames.Count} completed scenes.");
    }

    public void DeleteSave()
    {
        SaveSystem.Delete();
        CardInventory.Clear();
        completedSceneNames.Clear();
    }
}