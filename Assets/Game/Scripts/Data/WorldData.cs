using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Scriptable Objects/WorldData")]
public class WorldData : ScriptableObject
{
    public string[] scenes;
    public MedalToTime[] medalToTime;
}


[Serializable]
public struct MedalToTime
{
    public Medal medal;
    public float timeToCompleteWorld;
}