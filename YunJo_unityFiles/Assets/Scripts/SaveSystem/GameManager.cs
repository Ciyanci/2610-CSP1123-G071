using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Drag every CardData asset here")]
    public List<CardData> allCardData = new();

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

    public void SaveGame()
    {
        SaveData data = new SaveData();

        foreach (CardData card in CardInventory.GetAll()) //convert CardData objects into card names
        {
            data.unlockedCardNames.Add(card.Name);
        }

        SaveSystem.Save(data);

        Debug.Log($"[SAVE] Saved {data.unlockedCardNames.Count} cards");
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

        Debug.Log($"[LOAD] Loaded {CardInventory.GetAll().Count} cards");
    }

    public void DeleteSave()
    {
        SaveSystem.Delete();
        CardInventory.Clear();
    }
}