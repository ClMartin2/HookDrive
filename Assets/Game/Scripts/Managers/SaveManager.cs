using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Système de sauvegarde JSON basé sur PlayerPrefs.
/// Compatible WebGL / Poki.
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
        public string lastSelectedCar;
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
    /// Sauvegarde les données actuelles dans PlayerPrefs.
    /// </summary>
    public static void Save()
    {
        SerializableGameData s = new()
        {
            unlockedWorlds = Data.unlockedWorlds,
            unlockedCars = Data.unlockedCars,
            bestTimesKeys = new List<string>(Data.bestTimes.Keys),
            bestTimesValues = new List<float>(Data.bestTimes.Values),
            lastSelectedCar = Data.lastSelectedCar
        };

        string json = JsonUtility.ToJson(s);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Charge les données depuis PlayerPrefs.
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
            bestTimes = new Dictionary<string, float>(),
            lastSelectedCar = s.lastSelectedCar ?? string.Empty
        };

        if (s.bestTimesKeys != null && s.bestTimesValues != null)
        {
            for (int i = 0; i < s.bestTimesKeys.Count; i++)
                data.bestTimes[s.bestTimesKeys[i]] = s.bestTimesValues[i];
        }
    }

    /// <summary>
    /// Supprime complètement la sauvegarde.
    /// </summary>
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(Key);
        data = new GameData();
    }
}
