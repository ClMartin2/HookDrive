using UnityEngine;

/// <summary>
/// Script MonoBehaviour pour gérer sauvegarde automatique et debug.
/// Droppez sur un GameObject dans la scène.
/// </summary>
public class GameSaveController : MonoBehaviour
{
    public static GameSaveController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SaveManager.Load();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveManager.Save();
    }

    private void OnDisable()
    {
        SaveManager.Save();
    }

    #region API publiques pour le jeu

    // Débloquer un monde
    public void UnlockWorld(string worldName)
    {
        if (!SaveManager.Data.unlockedWorlds.Contains(worldName))
        {
            SaveManager.Data.unlockedWorlds.Add(worldName);
            SaveManager.Save();
        }
    }

    // Vérifier si un monde est débloqué
    public bool IsWorldUnlocked(string worldName)
    {
        return SaveManager.Data.unlockedWorlds.Contains(worldName);
    }

    // Enregistrer un meilleur temps
    public void SaveBestTime(string worldName, float time)
    {
        if (!SaveManager.Data.bestTimes.ContainsKey(worldName) || time < SaveManager.Data.bestTimes[worldName])
        {
            SaveManager.Data.bestTimes[worldName] = time;
            SaveManager.Save();
        }
    }

    // Récupérer un meilleur temps
    public float GetBestTime(string worldName)
    {
        if (SaveManager.Data.bestTimes.TryGetValue(worldName, out float time))
            return time;
        return -1f; // pas encore de temps
    }

    // Débloquer une voiture
    public void UnlockCar(string carName)
    {
        if (!SaveManager.Data.unlockedCars.Contains(carName))
        {
            SaveManager.Data.unlockedCars.Add(carName);
            SaveManager.Save();
        }
    }

    // Vérifier si une voiture est débloquée
    public bool IsCarUnlocked(string carName)
    {
        return SaveManager.Data.unlockedCars.Contains(carName);
    }

    [ContextMenu("Reset Save")]
    // Reset complet (debug)
    public void ResetSave()
    {
        SaveManager.ClearAll();
    }

    #endregion
}
