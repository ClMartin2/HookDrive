using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Lighting Preset")]
public class LightingPreset : ScriptableObject
{
    public LightmapData[] lightmaps;
    public LightProbes lightProbes;
    public Material skybox;
    public AmbientMode ambientMode;
    public Color ambientColor;
    public float ambientIntensity;
    public float reflectionIntensity;
    public SphericalHarmonicsL2 ambienProbe;

    public void Apply()
    {
        LightmapSettings.lightmaps = lightmaps;
        LightmapSettings.lightProbes = lightProbes;

        RenderSettings.skybox = skybox;
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.reflectionIntensity = reflectionIntensity;
        RenderSettings.ambientProbe = ambienProbe;

        DynamicGI.UpdateEnvironment();
    }
}
