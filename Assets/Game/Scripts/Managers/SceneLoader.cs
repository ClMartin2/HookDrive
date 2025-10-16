using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private StartLoadingScreen startLoadingScreen;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private float durationAnimLoadingScreen;

    public static SceneLoader Instance { get; private set; }

    private string currentScene;

    private void Awake()
    {
        loadingScreen.Hide();
        startLoadingScreen.Hide();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public async Task SwitchScene(string sceneToSwitch, bool startScene = false)
    {
        if (startScene)
            startLoadingScreen.Show();
        else
            loadingScreen.Show();

        await Task.Yield();

        if (currentScene != null)
            await SceneManager.UnloadSceneAsync(currentScene);

        // Lance le chargement async
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToSwitch, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        // Tant que la scène n'est pas prête à s'activer
        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            string progressText = (int)(progress * 100) + "%";

            // Mets à jour l'UI
            if (startScene)
                startLoadingScreen.txtLoading.text = progressText;
            else
            {
                loadingScreen.UpdateTextPercetage(progressText);
            }

            await Task.Yield(); // on attend la frame suivante
        }

        loadOperation.allowSceneActivation = true;

        // Attendre la fin complète du chargement
        while (!loadOperation.isDone)
            await Task.Yield();

        if (startScene)
            startLoadingScreen.Hide();
        else
        {
            loadingScreen.UpdateTextPercetage("");
            StartCoroutine(LoadingScreenAnimation());
        }

        currentScene = sceneToSwitch;
    }

    private IEnumerator LoadingScreenAnimation()
    {
        float counterAnimLoadingScreen = 0;

        while (counterAnimLoadingScreen < durationAnimLoadingScreen)
        {
            counterAnimLoadingScreen += Time.deltaTime;
            loadingScreen.UpdateBackgroundImage(1 - (counterAnimLoadingScreen / durationAnimLoadingScreen));
            yield return null;
        }

        loadingScreen.UpdateBackgroundImage(0);
        loadingScreen.Hide();

        yield return null;
    }
}
