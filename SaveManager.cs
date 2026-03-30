// 2. SaveManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;

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

        // Sets the save path to the OS's persistent data directory (works on PC, Mobile, Consoles)
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    public void SaveGame()
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
        File.WriteAllText(saveFilePath, finalJson);

        Debug.Log($"Game Saved Successfully to: {saveFilePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
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

        Debug.Log("Game Loaded Successfully!");
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data deleted.");
        }
    }
}
