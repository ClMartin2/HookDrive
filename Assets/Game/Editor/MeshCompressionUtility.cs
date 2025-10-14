using UnityEditor;
using UnityEngine;

public static class MeshCompressionUtility
{
    [MenuItem("Tools/Compress Selected Meshes")]
    public static void CompressSelectedMeshes()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Mesh mesh)
            {
                string path = AssetDatabase.GetAssetPath(mesh);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null)
                {
                    Debug.LogWarning($"Mesh {mesh.name} n’a pas d’importer (probablement généré).");
                    continue;
                }
                importer.meshCompression = ModelImporterMeshCompression.High;
                importer.SaveAndReimport();
                Debug.Log($"Compression appliquée sur {mesh.name}");
            }
        }
    }
}
