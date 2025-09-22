using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<WorldData> allWorlds = new();
    [SerializeField] private Menu menu;
    [SerializeField] private Hud hud;

    [Header("Debug"), Space(10)]
    [SerializeField] private bool loadMenu;
    [SerializeField] private WorldData startWorld;
    [SerializeField] private bool doNothing = false;

    public static GameManager Instance;

    private WorldData currentWorld;
    private int indexCurrentScene;
    private Player player;

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
        GameEvents.endScene += EndScene;

        Debug.Log("Awake called");

    }

    private IEnumerator Start()
    {
        Debug.Log("Start called");

        // Attendre une frame pour que le SDK Poki soit bien initialisé
        yield return new WaitForSeconds(0.1f);

#if UNITY_WEBGL && !UNITY_EDITOR
        if (PokiUnitySDK.Instance != null)
        {
            // Indique à Poki que le jeu a fini de charger
            PokiUnitySDK.Instance.gameLoadingFinished();
            Debug.Log("Poki: gameLoadingFinished called");
        }
        else
        {
            Debug.LogWarning("PokiUnitySDK.Instance is null, check JS plugin and template!");
        }
#endif

        // Préparer le player
        player = Player.Instance;
        player.Deactivate();

        // Si doNothing est true, on arrête ici
        if (doNothing)
            yield break;

        // Affiche menu ou charge directement le monde de départ
        if (loadMenu)
        {
            menu.Show();
            hud.Hide();
        }
        else
        {
            menu.Hide();
            hud.Show();
            LoadWorld(startWorld);
        }
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
        menu.Hide();
        hud.Show();
        LoadWorld(worldData);
        PokiUnitySDK.Instance.gameplayStart();
    }

    public void LoadWorld(WorldData worldData)
    {
        currentWorld = worldData;
        indexCurrentScene = 0;

        SceneLoader.Instance.SwitchScene(currentWorld.scenes[0]);
    }

    private void GoBackToMenu()
    {
        player.Deactivate();
        menu.Show();
        hud.Hide();
    }

    private void EndScene()
    {
        indexCurrentScene++;

        if (indexCurrentScene > currentWorld.scenes.Length - 1)
        {
            int currentWorldIndexData = allWorlds.FindIndex((worldData) => worldData == currentWorld);

            if (currentWorldIndexData < allWorlds.Count - 1)
            {
                currentWorldIndexData++;
                LoadWorld(allWorlds[currentWorldIndexData]);
            }
            else
            {
                GoBackToMenu();
            }
        }
        else
        {
            SceneLoader.Instance.SwitchScene(currentWorld.scenes[indexCurrentScene]);
        }
    }

    private void OnDestroy()
    {
        GameEvents.LoadWorld -= LoadWorldInMenu;
        GameEvents.endScene -= EndScene;
    }
}
