using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Scriptable Objects/WorldData")]
public class WorldData : ScriptableObject
{
    public string[] scenes;
    public MedalToTime[] medalToTime;
    public Medal actualMedal = Medal.none;
}


[Serializable]
public struct MedalToTime
{
    public Medal medal;
    public float timeToCompleteWorld;
}