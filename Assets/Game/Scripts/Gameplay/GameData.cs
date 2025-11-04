using System;
using System.Collections.Generic;

/// <summary>
/// Contient toutes les données sauvegardées du jeu.
/// </summary>
[Serializable]
public class GameData
{
    public List<string> unlockedWorlds = new();   // Mondes débloqués
    public Dictionary<string, float> bestTimes = new(); // Meilleurs temps par monde
    public List<string> unlockedCars = new();     // Voitures débloquées
    public string lastSelectedCar = string.Empty; // Dernière voiture sélectionnée
}
