using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] private WorldClearedScreen worldClearedScreen;
    [SerializeField] private WorldData startWorld;
    [SerializeField] private WorldData SAS;

    [Header("End level")]
    [SerializeField] private float timeToWaitEndLevel = 0.5f;
    [SerializeField] private float timeToWaitToShowWorldClearedScreen = 0.5f;
    [SerializeField] private float timeToWaitToSkipLevel = 0.1f;
    [SerializeField] private InputActionReference skipEndLevelInputs;

    [Header("Debug"), Space(10)]
    [SerializeField] private bool loadMenu;
    [SerializeField] private bool activateSAS;
    [SerializeField] private bool testLevel = false;
    [SerializeField] private bool mobileTest = false;

    public static GameManager Instance;
    public float timer { get; private set; }
    public bool gameplayStart { get; private set; } = false;
    public bool worldCleared { get; private set; } = false;

    private WorldData currentWorld;
    private int indexCurrentScene;
    private Player player;
    private Dictionary<WorldData, bool> unlocksWorldData = new();
    private int currentWorldUnlock = 1;
    private string currentScene;
    private Coroutine coroutineWaitToGoToNextLevel;
    private _Camera _camera;
    private bool stopTimer = false;

    private static bool _mobileTest;
    private ScreenOrientation currentScreenOrientation;
    private ScreenOrientation lastScreenOrientation;

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
        GameEvents.Play += PlayWorldCleared;
        GameEvents.OnRestartRequested += Restart;
        GameEvents.OnRestartWorld += RestartWorld;

        skipEndLevelInputs.action.performed += GoToNexLevel;

        for (int i = 0; i < allWorlds.Count; i++)
        {
            bool unlock = i < currentWorldUnlock;
            unlocksWorldData.Add(allWorlds[i], unlock);
        }

        _mobileTest = mobileTest;
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PokiUnitySDK.Instance.gameLoadingFinished();
#endif
        player = Player.Instance;

#if UNITY_EDITOR

        if (testLevel)
        {
            menu.Hide();
            hud.Show();
            return;
        }
#endif

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

#if UNITY_EDITOR
            if (activateSAS)
                _ = SceneLoader.Instance.SwitchScene(SAS.scenes[0], true);
            else
                LoadFirstWorld(true);
#else
            _ = SceneLoader.Instance.SwitchScene(SAS.scenes[0],true);
#endif

        }

        _camera = _Camera.Instance;
    }

    private void Update()
    {
        currentScreenOrientation = Screen.orientation;

        if (currentScreenOrientation != lastScreenOrientation)
            GameEvents.ChangeOrientation();

        lastScreenOrientation = currentScreenOrientation;

        if (!stopTimer)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);
        }

    }


    public static bool isMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile();
#else
        return _mobileTest;
#endif
    }

    public void LoadFirstWorld(bool startScene = false)
    {
        _ = LoadWorld(startWorld, startScene);
    }

    private void RestartWorld()
    {
        timer = 0;
        worldClearedScreen.Hide();
        _ = LoadWorld(currentWorld);
    }

    private void PlayWorldCleared()
    {
        GoToNexLevel();
    }

    private void Restart()
    {
        worldClearedScreen.Hide();
        hud.ActivateControlButtons(true);
        _camera.DeZoom();
        StopWaitToGoToNextLevel();
        stopTimer = false;
    }

    private void GameplayStart()
    {
        PokiUnitySDK.Instance.gameplayStart();
        gameplayStart = true;
    }

    private void LoadWorldInMenu(WorldData worldData)
    {
        hud.Show();
        menu.Hide();

        _ = LoadWorld(worldData);
    }

    private async Task LoadWorld(WorldData worldData, bool startScene = false)
    {
        currentWorld = worldData;
        indexCurrentScene = 0;
        currentScene = currentWorld.scenes[0];

        await SceneLoader.Instance.SwitchScene(currentScene, startScene);

        hud.UpdateLevelName(currentScene);
    }

    private void GoBackToMenu()
    {
        menu.Show();
        menu.SetLock(unlocksWorldData);

        player.Deactivate();
        hud.Hide();
    }

    [ContextMenu("World cleared")]
    private void WorldCleared()
    {
        EndScene(false);
    }

    private void EndScene(bool sas)
    {
        stopTimer = true;
        player.EndScene();
        _camera.Zoom();
        hud.ActivateControlButtons(false);
        coroutineWaitToGoToNextLevel = StartCoroutine(WaitEndScene(sas));
    }

    private IEnumerator WaitEndScene(bool sas)
    {
        yield return new WaitForSeconds(timeToWaitToSkipLevel);

        indexCurrentScene++;

        if (!sas)
        {
            if (indexCurrentScene > currentWorld.scenes.Length - 1)
            {
                yield return new WaitForSeconds(timeToWaitToShowWorldClearedScreen);
                worldClearedScreen.Show();
                worldClearedScreen.SetWorldClearedScreen(CheckTrophy(), currentScene);
                skipEndLevelInputs.action.Enable();
                worldCleared = true;
            }
            else
            {
                skipEndLevelInputs.action.Enable();
                yield return new WaitForSeconds(timeToWaitEndLevel);
                GoToNexLevel();
            }
        }
        else
        {
            yield return new WaitForSeconds(timeToWaitEndLevel);
            _ = GoToNextLevelAsync(sas);
        }

        yield return null;
    }

    private void GoToNexLevel(InputAction.CallbackContext obj = new InputAction.CallbackContext())
    {
        _ = GoToNextLevelAsync(false);
    }

    private async Task GoToNextLevelAsync(bool sas)
    {
        StopWaitToGoToNextLevel();

        if (!sas)
        {
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

                    await LoadWorld(worldToUnlock);
                }
                else
                {
                    await LoadWorld(allWorlds[0]);
                }
            }
            else
            {
                await SceneLoader.Instance.SwitchScene(currentWorld.scenes[indexCurrentScene]);
                currentScene = currentWorld.scenes[indexCurrentScene];
            }
        }
        else
        {
            LoadFirstWorld();
        }

        Restart();
        hud.UpdateLevelName(currentScene);
        worldCleared = false;
        stopTimer = false;
    }

    private void StopWaitToGoToNextLevel()
    {
        skipEndLevelInputs.action.Disable();

        if (coroutineWaitToGoToNextLevel != null)
        {
            StopCoroutine(coroutineWaitToGoToNextLevel);
            coroutineWaitToGoToNextLevel = null;
        }
    }

    private Medal CheckTrophy()
    {
        Medal actualMedal = Medal.bronze;

        foreach (MedalToTime medalToTime in currentWorld.medalToTime)
        {
            if (timer < medalToTime.timeToCompleteWorld)
            {
                actualMedal = medalToTime.medal;
            }

        }

        return actualMedal;
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
