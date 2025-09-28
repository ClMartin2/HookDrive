using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string currentScene;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public async void SwitchScene(string sceneToSwitch)
    {
        if(currentScene != null)
            await SceneManager.UnloadSceneAsync(currentScene);
        
        await SceneManager.LoadSceneAsync(sceneToSwitch,LoadSceneMode.Additive);

        currentScene = sceneToSwitch;       
    }
}
