using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action OnRestartRequested;
    public static Action<WorldData> LoadWorld;
    public static Action<bool> EndScene;
    public static Action GoBackToMenu;
    public static Action GameplayStart;
    public static Action ChangeOrientation;
}
