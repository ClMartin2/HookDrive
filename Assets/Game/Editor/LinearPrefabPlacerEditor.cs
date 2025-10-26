using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LinearPrefabPlacer))]
public class LinearPrefabPlacerEditor : Editor
{
    private LinearPrefabPlacer placer;

    // Pour détecter les changements
    private float lastPrefabLength;
    private float lastEndPoint;
    private Vector3 lastStartRotation;
    private Vector3 lastEndRotation;

    private void OnEnable()
    {
        placer = (LinearPrefabPlacer)target;

        lastPrefabLength = placer.prefabLength;
        lastEndPoint = placer.endPoint;
        lastStartRotation = placer.startRotation;
        lastEndRotation = placer.endRotation;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (placer.stopGenerate)
            return;

        // Si une valeur a changé → régénération
        if (placer.prefabLength != lastPrefabLength ||
            placer.endPoint != lastEndPoint ||
            placer.startRotation != lastStartRotation ||
            placer.endRotation != lastEndRotation)
        {
            GeneratePrefabs();
            lastPrefabLength = placer.prefabLength;
            lastEndPoint = placer.endPoint;
            lastStartRotation = placer.startRotation;
            lastEndRotation = placer.endRotation;
        }

        if (GUILayout.Button("Generate"))
        {
            GeneratePrefabs();
        }
    }

    private void GeneratePrefabs()
    {
        if (placer.prefab == null || placer.prefabLength <= 0f)
            return;

        // Supprimer les anciens enfants
        for (int i = placer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(placer.transform.GetChild(i).gameObject);
        }

        int count = Mathf.FloorToInt(placer.endPoint / placer.prefabLength);
        if (count <= 0)
            return;

        // Initialisation
        Vector3 dir = placer.direction != Vector3.zero ? placer.direction.normalized : Vector3.forward;
        Vector3 currentPos = Vector3.zero;
        Quaternion currentRot = Quaternion.LookRotation(dir);

        Quaternion previousRot = currentRot;

        for (int i = 0; i < count; i++)
        {
            // Interpolation progressive des rotations (en degrés)
            float t = count > 1 ? (float)i / (count - 1) : 0f;
            Vector3 interpolatedEuler = Vector3.Lerp(placer.startRotation, placer.endRotation, t);

            // Calcul de la rotation finale pour cette plateforme
            Quaternion finalRot = Quaternion.LookRotation(dir) * Quaternion.Euler(placer.offsetRotation + interpolatedEuler);

            // Position : pour la première, c’est l’origine
            if (i > 0)
            {
                // Calcul de la direction moyenne entre la rotation précédente et la nouvelle
                Quaternion midRot = Quaternion.Slerp(previousRot, finalRot, 0.5f);

                // On avance dans la direction "moyenne" → transition plus fluide
                currentPos += midRot * Vector3.forward * placer.prefabLength;
            }

            // Instanciation du prefab
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(placer.prefab, placer.transform);
            go.transform.localPosition = currentPos;
            go.transform.localRotation = finalRot;

            if (go.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
                sr.sortingOrder = 1;

            // Sauvegarder la rotation pour la prochaine itération
            previousRot = finalRot;
        }
    }

}
