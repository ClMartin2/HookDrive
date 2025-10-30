using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public List<string> unlockedWorlds = new();
    public Dictionary<string, float> bestTimes = new();
    public List<string> unlockedCars = new();

    public GameData() { }
}
