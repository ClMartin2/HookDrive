using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "WorldData", menuName = "Scriptable Objects/WorldData")]
public class WorldData : ScriptableObject
{
    public string[] scenes;
}
