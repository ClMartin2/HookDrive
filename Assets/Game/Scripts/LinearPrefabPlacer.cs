using UnityEngine;

[ExecuteAlways]
public class LinearPrefabPlacer : MonoBehaviour
{
    public GameObject prefab;
    public float prefabLength = 3f;
    public float endPoint = 100f;
    public Vector3 direction;
    public Vector3 offsetRotation;
}
