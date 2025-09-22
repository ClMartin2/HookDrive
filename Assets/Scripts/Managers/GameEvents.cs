using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action OnRestartRequested;
    public static Action<WorldData> LoadWorld;
    public static Action endScene;
}
