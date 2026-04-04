// 3. PlayerSaveExample.cs
using UnityEngine;

// Example to show how to implement ISaveable on an existing script
public class PlayerSaveExample : MonoBehaviour, ISaveable
{
    [SerializeField] private string uniqueSaveID = "Player_Stats";
    public string SaveID => uniqueSaveID;

    [Header("Player Data")]
    public int level = 1;
    public float health = 100f;
    public Vector3 playerPosition;

    // A private struct that holds ONLY the variables we want to save
    [System.Serializable]
    private struct PlayerData
    {
        public int level;
        public float health;
        public Vector3 position;
    }

    public string SaveState()
    {
        // Package our current variables into the struct
        PlayerData data = new PlayerData
        {
            level = this.level,
            health = this.health,
            position = transform.position
        };

        // Return it as a JSON string
        return JsonUtility.ToJson(data);
    }

    public void LoadState(string stateJson)
    {
        // Unpackage the JSON string back into our struct
        PlayerData data = JsonUtility.FromJson<PlayerData>(stateJson);

        // Apply it back to our actual variables (apply for all three, the level, the health and the position)
        this.level = data.level;
        this.health = data.health;
        transform.position = data.position;
    }
}
