using UnityEngine;

public class Decor : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;

    [Header("Lighting")]
    [SerializeField] private LightingPreset lightingPreset;

    //[SerializeField] private UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    //[SerializeField] private float ambientIntensity = 1f;
    //[SerializeField] private float reflectionIntensity = 1f;
    //[SerializeField] private Texture2D[] lightmaps;


    private void OnEnable()
    {
        RenderSettings.skybox = skyboxMaterial;
        ApplyLightingSettings();
    }


    public void ApplyLightmaps()
    {

    }

    private void ApplyLightingSettings()
    {
        if (lightingPreset != null)
            lightingPreset.Apply();

        //if (lightmaps.Length <= 0) return;

        //var data = new LightmapData[lightmaps.Length];

        //for (int i = 0; i < data.Length; i++)
        //{
        //    data[i] = new LightmapData();
        //    data[i].lightmapColor = lightmaps[i];
        //}

        //LightmapSettings.lightmaps = data;

        //RenderSettings.ambientMode = ambientMode;
        //RenderSettings.ambientIntensity = ambientIntensity;
        //RenderSettings.reflectionIntensity = reflectionIntensity;

        //DynamicGI.UpdateEnvironment();
    }
}
