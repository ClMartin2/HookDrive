#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshCompressionUtility
{
    [MenuItem("Tools/Compress All Merged Meshes")]
    public static void CompressMeshes()
    {
        string folder = "Assets/Game/MergedMeshes";
        string[] guids = AssetDatabase.FindAssets("t:Mesh", new[] { folder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

            if (mesh == null)
                continue;

            // Créer un importer temporaire pour appliquer la compression
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null)
            {
                importer.meshCompression = ModelImporterMeshCompression.Medium;
                importer.optimizeMeshPolygons = true;
                importer.optimizeMeshVertices = true;
                importer.SaveAndReimport();
                Debug.Log($"Compressed mesh: {mesh.name}");
            }
            else
            {
                Debug.LogWarning($"Mesh {mesh.name} has no importer (probably generated)");
            }
        }
    }
}
#endif
