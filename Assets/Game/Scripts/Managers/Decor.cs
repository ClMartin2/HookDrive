using UnityEditor;
using UnityEngine;

public class Decor : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;

    [Header("Lighting")]
    [SerializeField] private LightingDataAsset lightingData;
    [SerializeField] private UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    [SerializeField] private float ambientIntensity = 1f;
    [SerializeField] private float reflectionIntensity = 1f;

    private void OnEnable()
    {
        RenderSettings.skybox = skyboxMaterial;
        ApplyLightingSettings();
    }

    private void ApplyLightingSettings()
    {
        if (lightingData == null)
            return;

        Lightmapping.lightingDataAsset = lightingData;

        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.reflectionIntensity = reflectionIntensity;

        DynamicGI.UpdateEnvironment();
    }
}
