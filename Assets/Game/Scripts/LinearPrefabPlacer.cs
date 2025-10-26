using UnityEngine;

public class LinearPrefabPlacer : MonoBehaviour
{
    [Header("Base Prefab Settings")]
    public GameObject prefab;
    public float prefabLength = 1f;
    public float endPoint = 10f;
    public Vector3 direction = Vector3.forward;
    public Vector3 offsetRotation;

    [Header("Progressive Rotation")]
    public Vector3 startRotation = Vector3.zero;
    public Vector3 endRotation = Vector3.zero;

    [Header("Debug / Options")]
    public bool stopGenerate = false;
}
