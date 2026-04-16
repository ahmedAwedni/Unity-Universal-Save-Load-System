# Unity Universal Save & Load System

A clean, highly modular Save and Load system for Unity using JSON serialization. By leveraging a standard Interface, this system can save anything from player stats and inventory data to enemy positions and world states without requiring you to rewrite the core manager.

---

## ✨ Features

* **Multiple Save Slots:** Easily manage multiple playthroughs. The manager accepts an integer slot parameter to dynamically route data to "gamesave_1.json", "gamesave_2.json", etc.
* **Interface-Driven:** Simply add "ISaveable" to any script to include it in the save file. No need to hardcode specific classes into the Save Manager.
* **JSON Serialization:** Uses Unity's built-in "JsonUtility" for lightweight, readable save files.
* **Cross-Platform:** Automatically saves to "Application.persistentDataPath", ensuring it works flawlessly on PC, Mac, iOS, Android, and Consoles.
* **No Dependencies:** Doesn't require third-party plugins. Pure C# and Unity API.

---

## 🧠 Design Notes

Writing a custom save function for every single script in your game creates a massive, tangled "SaveManager" that breaks easily. 

This system uses the **Dependency Inversion** principle. The "SaveManager" doesn't know *what* a Player or a Treasure Chest is. It only knows about the "ISaveable" interface. When you call "SaveGame()", the manager politely asks every "ISaveable" object in the scene to pack up its own data into a JSON string. The manager then acts as a courier, bundling those strings together and writing them to the correct slot on your hard drive.

---

## 📂 Included Scripts

* "ISaveable.cs" - The interface contract that any script must implement if it wants its data saved.
* "SaveManager.cs" - The core singleton that handles dynamic slot routing, reading, and writing the final JSON file to your local disk.
* "PlayerSaveExample.cs" - A fully commented example script demonstrating how to easily pack and unpack variables like health, level, and Vector3 positions.

---

## 🧩 How To Use

1. **Setup the Manager:** Create an empty GameObject in your scene named "SaveManager" and attach the "SaveManager.cs" script.
2. **Implement ISaveable:** Open any script you want to save (e.g., your Player script) and add ", ISaveable" next to "MonoBehaviour".
3. **Add the required Methods:** Implement the "SaveID" property, and the "SaveState()" and "LoadState()" methods (see "PlayerSaveExample.cs" for the exact syntax).
4. **Trigger a Save (Default Slot 1):** Call the singleton from any button or event:
   
"
SaveManager.Instance.SaveGame();
"

5. **Trigger a Save (Specific Slot):** Great for multiple save files:

"
SaveManager.Instance.SaveGame(3); // Saves to gamesave_3.json
"

6. **Trigger a Load:**
   
"
SaveManager.Instance.LoadGame(3); // Loads from gamesave_3.json
"

7. **Check if Save Exists:** Perfect for verifying if a "Continue" UI button should be active:
    
"
bool hasSave = SaveManager.Instance.DoesSaveExist(3);
"

---

## 🚀 Possible Extensions

* **Encryption:** Add a simple XOR encryption method or Base64 encoding to the final JSON string before writing it to the file to prevent players from easily editing their stats.
* **Binary Formatter:** Swap out "JsonUtility" for a Binary writer if you need maximum file compression for massive simulation games.
* **Save Metadata:** Create a separate lightweight "SaveHeader.json" system that saves just the Player's Level and Playtime. Load *only* the headers for your Main Menu UI so you don't have to load the entire world state just to show the save slots.

---

## 🛠 Unity Version

Tested in Unity6+ (should work without any problems in newer versions)

---

## 📜 License

MIT
