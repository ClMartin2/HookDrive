using UnityEngine;

/// <summary>
/// Contrôleur central de la sauvegarde.
/// Droppez ce script sur un GameObject dans la scène.
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
        DontDestroyOnLoad(gameObject); // Persiste entre les scènes
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

    #region === MONDES ===

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

    #endregion

    #region === TEMPS ===

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
        return -1f;
    }

    #endregion

    #region === VOITURES ===

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

    // Définir la dernière voiture sélectionnée
    public void SetLastSelectedCar(string carName)
    {
        SaveManager.Data.lastSelectedCar = carName;
        SaveManager.Save();
    }

    // Obtenir la dernière voiture sélectionnée
    public string GetLastSelectedCar()
    {
        return SaveManager.Data.lastSelectedCar;
    }

    #endregion

    #region === DEBUG ===

    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        SaveManager.ClearAll();
    }

    #endregion
}
