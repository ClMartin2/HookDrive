using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using System.Collections.Generic;
using System.IO;

public class BatchFBXExporter : EditorWindow
{
    private List<GameObject> fbxObjects = new List<GameObject>();
    private string outputFolder = "";

    [MenuItem("Tools/Batch FBX Exporter")]
    public static void ShowWindow()
    {
        GetWindow<BatchFBXExporter>("Batch FBX Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch FBX Exporter", EditorStyles.boldLabel);

        // Ajouter objets sélectionnés
        if (GUILayout.Button("Add Selected Objects"))
        {
            AddSelectedObjects();
        }

        if (fbxObjects.Count > 0)
        {
            GUILayout.Label($"Objects to Export: {fbxObjects.Count}");
            for (int i = 0; i < fbxObjects.Count; i++)
            {
                GUILayout.Label($"{i + 1}. {fbxObjects[i].name}");
            }

            if (GUILayout.Button("Clear List"))
            {
                fbxObjects.Clear();
            }
        }

        GUILayout.Space(10);

        // Choisir dossier de sortie
        GUILayout.Label("Output Folder", EditorStyles.boldLabel);
        if (GUILayout.Button("Select Output Folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Output Folder", "", "");
            if (!string.IsNullOrEmpty(folder))
            {
                outputFolder = folder;
            }
        }
        GUILayout.Label(outputFolder);

        GUILayout.Space(10);

        // Exporter
        if (GUILayout.Button("Export All FBX"))
        {
            ExportAll();
        }
    }

    private void AddSelectedObjects()
    {
        foreach (var obj in Selection.gameObjects)
        {
            if (!fbxObjects.Contains(obj))
            {
                fbxObjects.Add(obj);
            }
        }
    }

    private void ExportAll()
    {
        if (fbxObjects.Count == 0)
        {
            Debug.LogWarning("No objects to export!");
            return;
        }

        if (string.IsNullOrEmpty(outputFolder))
        {
            Debug.LogError("Please select an output folder first!");
            return;
        }

        foreach (var obj in fbxObjects)
        {
            string path = Path.Combine(outputFolder, obj.name + ".fbx");

            // Crée les options pour forcer le binaire
            var exportOptions = new ExportModelOptions();
            exportOptions.ExportFormat = ExportFormat.Binary; // Force Binary

            // Exporte un objet à la fois
            ModelExporter.ExportObjects(path, new GameObject[] { obj }, exportOptions);

            Debug.Log($"Exported (binary): {path}");
        }

        Debug.Log("✅ Batch export finished!");
    }
}
