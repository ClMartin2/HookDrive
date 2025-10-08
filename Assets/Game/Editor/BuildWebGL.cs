using System.Linq;
using UnityEditor;

public static class BuildWebGL
{
    public static void Build()
    {
        // Profil de build : Web-poki
        string buildPath = "Builds/Web-poki";

        // Sélection des scènes à build (toutes celles actives dans Build Settings)
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        // Lancer la build WebGL
        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.WebGL,
            BuildOptions.None
        );
    }
}
