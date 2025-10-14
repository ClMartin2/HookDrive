using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class MeshMerger : MonoBehaviour
{
    [Tooltip("If true the original child GameObjects will be deactivated after merging. If false they will be left as-is.")]
    [SerializeField] private bool deactivateChildren = true;

    [Tooltip("If true the original child GameObjects will be destroyed after merging. (Takes precedence over deactivateChildren)")]
    [SerializeField] private bool destroyChildren = false;

    [Tooltip("Folder path where the merged meshes will be saved.")]
    [SerializeField] private string saveFolder = "Assets/Game/MergedMeshes";

    [ContextMenu("Merge Meshes")]
    public void MergeMeshes()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        if (filters == null || filters.Length == 0)
        {
            Debug.LogWarning("[MeshMerger] No MeshFilters found under " + gameObject.name);
            return;
        }

        // Création du GO qui contiendra le mesh fusionné
        GameObject mergedGO = new GameObject("MergedMesh");
        mergedGO.transform.SetParent(transform, false);
        mergedGO.transform.localPosition = Vector3.zero;
        mergedGO.transform.localRotation = Quaternion.identity;
        mergedGO.transform.localScale = Vector3.one;

        Dictionary<Material, List<CombineInstance>> combineDict = new Dictionary<Material, List<CombineInstance>>();

        int meshCount = 0;
        int skipped = 0;

        Matrix4x4 mergedWorldToLocal = mergedGO.transform.worldToLocalMatrix;

        foreach (MeshFilter mf in filters)
        {
            if (mf.gameObject == mergedGO) continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null || mf.sharedMesh == null)
            {
                skipped++;
                continue;
            }

            Mesh srcMesh = mf.sharedMesh;
            meshCount++;

            Material[] mats = mr.sharedMaterials;
            for (int sub = 0; sub < mats.Length; sub++)
            {
                Material mat = mats[sub];
                if (mat == null) continue;

                if (!combineDict.TryGetValue(mat, out var list))
                {
                    list = new List<CombineInstance>();
                    combineDict[mat] = list;
                }

                CombineInstance ci = new CombineInstance
                {
                    mesh = srcMesh,
                    subMeshIndex = sub,
                    transform = mergedWorldToLocal * mf.transform.localToWorldMatrix
                };
                list.Add(ci);
            }

            if (destroyChildren)
                DestroyImmediate(mf.gameObject);
            else if (deactivateChildren)
                mf.gameObject.SetActive(false);
        }

        if (combineDict.Count == 0)
        {
            Debug.LogWarning("[MeshMerger] Nothing to combine (no materials/meshes).");
            return;
        }

        List<CombineInstance> finalCombine = new List<CombineInstance>();
        Material[] finalMaterials = new Material[combineDict.Count];
        int matIndex = 0;

        foreach (var kvp in combineDict)
        {
            Material mat = kvp.Key;
            List<CombineInstance> instances = kvp.Value;

            Mesh subMesh = new Mesh();
            subMesh.name = "SubMesh_" + matIndex;
            subMesh.CombineMeshes(instances.ToArray(), true, true);

            CombineInstance ci = new CombineInstance
            {
                mesh = subMesh,
                subMeshIndex = 0,
                transform = Matrix4x4.identity
            };
            finalCombine.Add(ci);

            finalMaterials[matIndex] = mat;
            matIndex++;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.name = "MergedMesh_Final";
        finalMesh.CombineMeshes(finalCombine.ToArray(), false, false);

        MeshFilter mergedMF = mergedGO.AddComponent<MeshFilter>();
        mergedMF.sharedMesh = finalMesh;

        MeshRenderer mergedMR = mergedGO.AddComponent<MeshRenderer>();
        mergedMR.sharedMaterials = finalMaterials;

#if UNITY_EDITOR
        // Création du dossier si inexistant
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        // Génération d'un nom unique si le mesh existe déjà
        string assetName = $"{finalMesh.name}_{gameObject.name}";
        string assetPath = Path.Combine(saveFolder, assetName + ".asset");
        int counter = 1;
        while (File.Exists(assetPath))
        {
            assetPath = Path.Combine(saveFolder, $"{assetName}_{counter}.asset");
            counter++;
        }

        // Création ou mise à jour de l'asset
        AssetDatabase.CreateAsset(finalMesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        mergedMF.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        Debug.Log($"✅ [MeshMerger] Merge complete — Saved mesh at: {assetPath}");
#endif

        Debug.LogFormat("[MeshMerger] Merged {0} meshes into {1} materials. Skipped {2} entries. Result GameObject: {3}",
            meshCount, finalMaterials.Length, skipped, mergedGO.name);
    }
}
