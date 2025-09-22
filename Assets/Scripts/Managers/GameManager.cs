using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<WorldData> allWorlds = new();
    [SerializeField] private Menu menu;
    [SerializeField] private Hud hud;

    public static GameManager Instance;

    private WorldData currentWorld;
    private int indexCurrentScene;
    private Player player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GameEvents.LoadWorld += LoadWorldInMenu;
        GameEvents.endScene += EndScene;
    }

    private void Start()
    {
        player = Player.Instance;
        player.Deactivate();
    }

    private void LoadWorldInMenu(WorldData worldData)
    {
        menu.Hide();
        hud.Show();
        LoadWorld(worldData);
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

        if(indexCurrentScene > currentWorld.scenes.Length - 1)
        {
            int currentWorldIndexData = allWorlds.FindIndex((worldData) => worldData == currentWorld);

            if(currentWorldIndexData < allWorlds.Count - 1)
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
