using UnityEngine;
using System.Collections.Generic;

public class MeshMerger : MonoBehaviour
{
    [ContextMenu("Merge Meshes")]
    public void MergeMeshes()
    {
        // R�cup�rer tous les MeshRenderer et MeshFilter enfants
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        // Dictionnaire pour fusionner par mat�riau
        Dictionary<Material, List<CombineInstance>> combineDict = new Dictionary<Material, List<CombineInstance>>();

        // Cr�er le GameObject pour le mesh fusionn�
        GameObject mergedGO = new GameObject("MergedMesh");
        mergedGO.transform.position = transform.position;
        mergedGO.transform.rotation = transform.rotation;
        mergedGO.transform.localScale = transform.localScale;
        mergedGO.transform.parent = transform;

        foreach (MeshFilter mf in filters)
        {
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null || mf.sharedMesh == null)
                continue;

            // Pour chaque mat�riau du mesh
            for (int i = 0; i < mr.sharedMaterials.Length; i++)
            {
                Material mat = mr.sharedMaterials[i];
                if (!combineDict.ContainsKey(mat))
                    combineDict[mat] = new List<CombineInstance>();

                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.subMeshIndex = i;

                // Correction pivot / position
                ci.transform = mf.transform.localToWorldMatrix * mergedGO.transform.worldToLocalMatrix;

                combineDict[mat].Add(ci);
            }

            // D�sactiver l'ancien mesh
            mf.gameObject.SetActive(false);
        }

        // Fusionner les meshes par mat�riau
        List<CombineInstance> finalCombine = new List<CombineInstance>();
        Material[] materials = new Material[combineDict.Keys.Count];
        int matIndex = 0;

        foreach (var kvp in combineDict)
        {
            Material mat = kvp.Key;
            List<CombineInstance> instances = kvp.Value;

            Mesh subMesh = new Mesh();
            subMesh.name = "SubMesh_" + matIndex;
            subMesh.CombineMeshes(instances.ToArray(), true, true);

            CombineInstance ci = new CombineInstance();
            ci.mesh = subMesh;
            ci.transform = Matrix4x4.identity; // submesh d�j� dans le bon rep�re
            finalCombine.Add(ci);

            materials[matIndex] = mat;
            matIndex++;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.name = "MergedMesh_Final";
        finalMesh.CombineMeshes(finalCombine.ToArray(), false, false);

        // Ajouter les composants au GameObject fusionn�
        MeshFilter mergedMF = mergedGO.AddComponent<MeshFilter>();
        mergedMF.sharedMesh = finalMesh;

        MeshRenderer mergedMR = mergedGO.AddComponent<MeshRenderer>();
        mergedMR.sharedMaterials = materials;

        // Activer Static Batching
        StaticBatchingUtility.Combine(mergedGO);

        Debug.Log("Meshes merged and aligned for " + gameObject.name);
    }
}
