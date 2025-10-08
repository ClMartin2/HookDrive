using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<WorldData> allWorlds = new();
    [SerializeField] private Menu menu;
    [SerializeField] private Hud hud;

    [Header("End level")]
    [SerializeField] private float timeToWaitEndLevel = 0.5f;
    [SerializeField] private float timeToWaitToSkipLevel = 0.1f;
    [SerializeField] private InputActionReference skipEndLevelInputs;

    [Header("Debug"), Space(10)]
    [SerializeField] private bool loadMenu;
    [SerializeField] private WorldData startWorld;
    [SerializeField] private bool testLevel = false;

    public static GameManager Instance;
    public bool gameplayStart { get; private set; } = false;

    private WorldData currentWorld;
    private int indexCurrentScene;
    private Player player;
    private Dictionary<WorldData, bool> unlocksWorldData = new();
    private int currentWorldUnlock = 1;
    private string currentScene;
    private Coroutine coroutineWaitToGoToNextLevel;
    private _Camera _camera;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GameEvents.LoadWorld += LoadWorldInMenu;
        GameEvents.EndScene += EndScene;
        GameEvents.GoBackToMenu += GoBackToMenu;
        GameEvents.GameplayStart += GameplayStart;

        skipEndLevelInputs.action.performed += GoToNexLevel;

        for (int i = 0; i < allWorlds.Count; i++)
        {
            bool unlock = i < currentWorldUnlock;
            unlocksWorldData.Add(allWorlds[i], unlock);
        }
    }

    private void GameplayStart()
    {
        PokiUnitySDK.Instance.gameplayStart();
        gameplayStart = true;
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PokiUnitySDK.Instance.gameLoadingFinished();
#endif
        player = Player.Instance;

        if (testLevel)
        {
            menu.Hide();
            hud.Show();
            return;
        }

        if (loadMenu)
        {
            menu.Show();
            menu.SetLock(unlocksWorldData);
            hud.Hide();
            player.Deactivate();
        }
        else
        {
            hud.Show();
            menu.Hide();
            LoadWorld(startWorld);
        }

        _camera = _Camera.Instance;
    }

    public static bool isMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobile();
#else
        return false;
#endif
    }

    private void LoadWorldInMenu(WorldData worldData)
    {
        hud.Show();
        menu.Hide();

        LoadWorld(worldData);
        PokiUnitySDK.Instance.gameplayStart();
    }

    public void LoadWorld(WorldData worldData)
    {
        currentWorld = worldData;
        indexCurrentScene = 0;
        currentScene = currentWorld.scenes[0];

        SceneLoader.Instance.SwitchScene(currentScene);

        hud.UpdateLevelName(currentScene);
    }

    private void GoBackToMenu()
    {
        menu.Show();
        menu.SetLock(unlocksWorldData);

        player.Deactivate();
        hud.Hide();
    }

    private void EndScene()
    {
        player.EndScene();
        _camera.Zoom();
        coroutineWaitToGoToNextLevel = StartCoroutine(WaitEndScene());
    }

    private IEnumerator WaitEndScene()
    {
        yield return new WaitForSeconds(timeToWaitToSkipLevel);
        skipEndLevelInputs.action.Enable();
        yield return new WaitForSeconds(timeToWaitEndLevel);
        GoToNexLevel();
        yield return null;
    }

    private void GoToNexLevel(InputAction.CallbackContext obj = new InputAction.CallbackContext())
    {
        if (coroutineWaitToGoToNextLevel != null)
        {
            StopCoroutine(coroutineWaitToGoToNextLevel);
            coroutineWaitToGoToNextLevel = null;
        }

        indexCurrentScene++;

        if (indexCurrentScene > currentWorld.scenes.Length - 1)
        {
            //Check si le monde est deja débloqué
            int currentWorldIndexData = allWorlds.FindIndex((worldData) => worldData == currentWorld);

            if (currentWorldIndexData < allWorlds.Count - 1)
            {
                currentWorldIndexData++;
                currentWorldUnlock++;

                WorldData worldToUnlock = allWorlds[currentWorldIndexData];
                unlocksWorldData[worldToUnlock] = true;

                LoadWorld(worldToUnlock);
            }
            else
            {
                //GoBackToMenu();
                LoadWorld(allWorlds[0]);
            }
        }
        else
        {
            SceneLoader.Instance.SwitchScene(currentWorld.scenes[indexCurrentScene]);
            currentScene = currentWorld.scenes[indexCurrentScene];
        }

        _camera.DeZoom();
        skipEndLevelInputs.action.Disable();
        hud.UpdateLevelName(currentScene);
    }

    private void OnDestroy()
    {
        GameEvents.LoadWorld -= LoadWorldInMenu;
        GameEvents.EndScene -= EndScene;
        GameEvents.GoBackToMenu -= GoBackToMenu;
        GameEvents.GameplayStart -= GameplayStart;

        skipEndLevelInputs.action.performed -= GoToNexLevel;
    }
}
