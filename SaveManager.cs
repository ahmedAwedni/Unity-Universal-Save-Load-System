// 2. SaveManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // A wrapper class to hold our dictionary data since JsonUtility doesn't support Dictionaries natively
    [System.Serializable]
    private class SaveDataWrapper
    {
        public List<string> saveIDs = new List<string>();
        public List<string> jsonStates = new List<string>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Helper method to dynamically generate the file path for a specific slot
    private string GetSaveFilePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"gamesave_{slot}.json");
    }


    /// Checks if a save file exists in the specified slot. Useful for UI "Continue" buttons.

    public bool DoesSaveExist(int slot = 1)
    {
        return File.Exists(GetSaveFilePath(slot));
    }

    public void SaveGame(int slot = 1)
    {
        // Find all objects in the active scene that implement ISaveable
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        
        SaveDataWrapper wrapper = new SaveDataWrapper();

        foreach (ISaveable saveable in saveables)
        {
            wrapper.saveIDs.Add(saveable.SaveID);
            wrapper.jsonStates.Add(saveable.SaveState());
        }

        string finalJson = JsonUtility.ToJson(wrapper, true);
        string currentSavePath = GetSaveFilePath(slot);
        
        File.WriteAllText(currentSavePath, finalJson);

        Debug.Log($"Game Saved Successfully to Slot {slot}: {currentSavePath}");
    }

    public void LoadGame(int slot = 1)
    {
        string currentSavePath = GetSaveFilePath(slot);

        if (!File.Exists(currentSavePath))
        {
            Debug.LogWarning($"No save file found in Slot {slot}!");
            return;
        }

        string json = File.ReadAllText(currentSavePath);
        SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

        // Rebuild a dictionary for quick lookups
        Dictionary<string, string> savedStates = new Dictionary<string, string>();
        for (int i = 0; i < wrapper.saveIDs.Count; i++)
        {
            savedStates.Add(wrapper.saveIDs[i], wrapper.jsonStates[i]);
        }

        // Apply data to active objects
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        foreach (ISaveable saveable in saveables)
        {
            if (savedStates.TryGetValue(saveable.SaveID, out string savedJson))
            {
                saveable.LoadState(savedJson);
            }
        }

        Debug.Log($"Game Loaded Successfully from Slot {slot}!");
    }

    public void DeleteSaveData(int slot = 1)
    {
        string currentSavePath = GetSaveFilePath(slot);
        
        if (File.Exists(currentSavePath))
        {
            File.Delete(currentSavePath);
            Debug.Log($"Save data in Slot {slot} deleted.");
        }
    }
}
