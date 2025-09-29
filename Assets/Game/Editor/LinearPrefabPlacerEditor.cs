using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LinearPrefabPlacer))]
public class LinearPrefabPlacerEditor : Editor
{
    private LinearPrefabPlacer placer;
    private float lastPrefabLength;
    private float lastEndPoint;

    private void OnEnable()
    {
        placer = (LinearPrefabPlacer)target;
        lastPrefabLength = placer.prefabLength;
        lastEndPoint = placer.endPoint;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Si valeurs changent, régénérer
        if (placer.prefabLength != lastPrefabLength || placer.endPoint != lastEndPoint)
        {
            GeneratePrefabs();
            lastPrefabLength = placer.prefabLength;
            lastEndPoint = placer.endPoint;
        }

        if (GUILayout.Button("Generate"))
        {
            GeneratePrefabs();
        }
    }

    private void GeneratePrefabs()
    {
        if (placer.prefab == null || placer.prefabLength <= 0f) return;

        // Supprimer les enfants
        for (int i = placer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(placer.transform.GetChild(i).gameObject);
        }

        int count = Mathf.FloorToInt(placer.endPoint / placer.prefabLength);
        if (count <= 0) return;

        Vector3 dir = Vector3.forward;

        for (int i = 0; i < count; i++)
        {
            float distance = i * placer.prefabLength;
            Vector3 localPos = dir * distance;
            Quaternion rot = Quaternion.LookRotation(dir);

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(placer.prefab, placer.transform);
            go.transform.localPosition = localPos;
            go.transform.localRotation = rot;
        }
    }
}
