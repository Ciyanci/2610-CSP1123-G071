using UnityEngine;
using System.IO;

public static class SaveSystem
{
    static string savePath => Application.persistentDataPath + "save/json";

    public static void Save(SaveData data) //saves data to save.json
    {
        string json = JsonUtility.ToJson(data, true); // convert SaveData into json

        File.WriteAllText(savePath, json); //basically writes json to the save file

        Debug.Log($"[SAVE] Saved to {savePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("[SAVE] No save file found, starting fresh");
            return new SaveData();
        }

        string json = File.ReadAllText(savePath); 

        return JsonUtility.FromJson<SaveData>(json); //convert json to SaveData object
    }

    public static void Delete()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        Debug.Log("[SAVE] Save Deleted");
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}
