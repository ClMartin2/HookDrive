using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Scene startScene = Scene.W1_Level1;

    public static SceneLoader Instance { get; private set; }

    private Scene currentScene = Scene.PERSISTENT;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SwitchScene(startScene);
    }

    public async void SwitchScene(Scene sceneToSwitch)
    {
        if(currentScene != Scene.PERSISTENT)
            await SceneManager.UnloadSceneAsync((int)currentScene);
        
        await SceneManager.LoadSceneAsync((int)sceneToSwitch,LoadSceneMode.Additive);

        currentScene = sceneToSwitch;       
    }
}

public enum Scene
{
    PERSISTENT,
    W1_Level1,
    W2_Level2
}
