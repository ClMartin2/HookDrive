#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class LightingPresetCreator
{
    [MenuItem("Tools/Create Lighting Preset from Scene")]
    static void CreatePreset()
    {
        LightingPreset preset = ScriptableObject.CreateInstance<LightingPreset>();

        preset.lightmaps = LightmapSettings.lightmaps;
        preset.lightProbes = LightmapSettings.lightProbes;
        preset.skybox = RenderSettings.skybox;
        preset.ambientMode = RenderSettings.ambientMode;
        preset.ambientColor = RenderSettings.ambientLight;
        preset.ambientIntensity = RenderSettings.ambientIntensity;
        preset.reflectionIntensity = RenderSettings.reflectionIntensity;
        preset.ambienProbe = RenderSettings.ambientProbe;

        AssetDatabase.CreateAsset(preset, "Assets/Game/LightingPresets/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_Lighting.asset");
        AssetDatabase.SaveAssets();
    }
}
#endif
