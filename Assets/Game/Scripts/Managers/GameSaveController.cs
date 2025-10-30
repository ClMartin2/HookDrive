using UnityEngine;

/// <summary>
/// Script MonoBehaviour pour g�rer sauvegarde automatique et debug.
/// Droppez sur un GameObject dans la sc�ne.
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

    // D�bloquer un monde
    public void UnlockWorld(string worldName)
    {
        if (!SaveManager.Data.unlockedWorlds.Contains(worldName))
        {
            SaveManager.Data.unlockedWorlds.Add(worldName);
            SaveManager.Save();
        }
    }

    // V�rifier si un monde est d�bloqu�
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

    // R�cup�rer un meilleur temps
    public float GetBestTime(string worldName)
    {
        if (SaveManager.Data.bestTimes.TryGetValue(worldName, out float time))
            return time;
        return -1f; // pas encore de temps
    }

    // D�bloquer une voiture
    public void UnlockCar(string carName)
    {
        if (!SaveManager.Data.unlockedCars.Contains(carName))
        {
            SaveManager.Data.unlockedCars.Add(carName);
            SaveManager.Save();
        }
    }

    // V�rifier si une voiture est d�bloqu�e
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
