using UnityEngine;

public class Decor : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;

    private void Start()
    {
        RenderSettings.skybox = skyboxMaterial;
        //DynamicGI.UpdateEnvironment();
    }
}
