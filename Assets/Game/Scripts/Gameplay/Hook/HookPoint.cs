using UnityEngine;

public class HookPoint : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material materialUnlock;
    [SerializeField] private Material materialLock;

    private void Start()
    {
        Unlock();
    }

    public void Lock()
    {
        meshRenderer.material = materialLock;
    }

    public void Unlock()
    {
        meshRenderer.material = materialUnlock;
    }
}
