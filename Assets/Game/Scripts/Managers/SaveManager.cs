using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Système de sauvegarde JSON sur WebGL (PlayerPrefs / LocalStorage)
/// Extensible pour Poki.
/// </summary>
public static class SaveManager
{
    private const string Key = "GameData";

    [System.Serializable]
    private class SerializableGameData
    {
        public List<string> unlockedWorlds;
        public List<string> unlockedCars;
        public List<string> bestTimesKeys;
        public List<float> bestTimesValues;
    }

    private static GameData data;

    public static GameData Data
    {
        get
        {
            if (data == null)
                Load();
            return data;
        }
    }

    /// <summary>
    /// Sauvegarde dans PlayerPrefs
    /// </summary>
    public static void Save()
    {
        SerializableGameData s = new()
        {
            unlockedWorlds = Data.unlockedWorlds,
            unlockedCars = Data.unlockedCars,
            bestTimesKeys = new List<string>(Data.bestTimes.Keys),
            bestTimesValues = new List<float>(Data.bestTimes.Values)
        };

        string json = JsonUtility.ToJson(s);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Charge depuis PlayerPrefs
    /// </summary>
    public static void Load()
    {
        if (!PlayerPrefs.HasKey(Key))
        {
            data = new GameData();
            return;
        }

        string json = PlayerPrefs.GetString(Key);
        SerializableGameData s = JsonUtility.FromJson<SerializableGameData>(json);

        data = new GameData
        {
            unlockedWorlds = s.unlockedWorlds ?? new(),
            unlockedCars = s.unlockedCars ?? new(),
            bestTimes = new Dictionary<string, float>()
        };

        if (s.bestTimesKeys != null && s.bestTimesValues != null)
        {
            for (int i = 0; i < s.bestTimesKeys.Count; i++)
                data.bestTimes[s.bestTimesKeys[i]] = s.bestTimesValues[i];
        }
    }

    /// <summary>
    /// Supprime toutes les données
    /// </summary>
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(Key);
        data = new GameData();
    }
}
