using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    private const string SAVE_KEY = "EchoNomad_CollectedItems";

    /// <summary>
    /// Save data structure
    /// </summary>
    [System.Serializable]
    private class SaveData
    {
        public List<string> collectedIDs = new List<string>();
        private const string CURRENT_VERSION = "1.0.1";
    }

    /// <summary>
    /// Mark a collectible as collected
    /// </summary>
    public static void MarkCollected(string collectibleID)
    {
        SaveData data = Load();

        if (!data.collectedIDs.Contains(collectibleID))
        {
            data.collectedIDs.Add(collectibleID);
            Save(data);
        }
    }

    /// <summary>
    /// Check if a collectible is already collected
    /// </summary>
    public static bool IsCollected(string collectibleID)
    {
        SaveData data = Load();
        return data.collectedIDs.Contains(collectibleID);
    }

    /// <summary>
    /// Get all collected IDs
    /// </summary>
    public static List<string> GetAllCollected()
    {
        SaveData data = Load();
        return new List<string>(data.collectedIDs);
    }

    /// <summary>
    /// Clear all save data (for testing)
    /// </summary>
    public static void ClearAllData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("SaveSystem: All data cleared");
    }

    /// <summary>
    /// Get total collected count
    /// </summary>
    public static int GetCollectedCount()
    {
        SaveData data = Load();
        return data.collectedIDs.Count;
    }

    // Private methods
    private static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private static SaveData Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData();
        }
    }
}