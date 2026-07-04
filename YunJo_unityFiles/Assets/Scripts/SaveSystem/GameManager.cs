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

    [Header("Visual Novel")]
    public StoryScene targetScene;
    public int levelSelectionSceneId = 11; // ← new

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

    public void SetTargetScene(StoryScene scene)
    {
        targetScene = scene;
        Debug.Log($"[GameManager] Target scene set to: {scene.name}");
    }

    public void ReturnToLevelSelection() // ← new
    {
        LoadingBar loadingBar = FindObjectOfType<LoadingBar>(true);
        if (loadingBar == null)
        {
            Debug.LogError("[GameManager] LoadingBar not found!");
            return;
        }
        loadingBar.LoadScene(levelSelectionSceneId);
    }

    public void UnlockCard(CardData card)
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

        foreach (CardData card in CardInventory.GetAll())
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

        foreach (string name in data.unlockedCardNames)
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