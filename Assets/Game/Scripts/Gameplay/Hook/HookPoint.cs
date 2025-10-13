using UnityEngine;

public class HookPoint : MonoBehaviour
{
    [SerializeField] private GameObject unlockMesh;
    [SerializeField] private GameObject lockMesh;
    [SerializeField] private GameObject onMesh;

    private void Start()
    {
        Unlock();
        UnAttached();
    }

    public void Lock()
    {
        onMesh.SetActive(true);
    }

    public void Unlock()
    {
        onMesh.SetActive(false);
    }

    public void Attached()
    {
        ActivateLockMesh(true);
    }

    public void UnAttached()
    {
        ActivateLockMesh(false);
    }

    private void ActivateLockMesh(bool Activate)
    {
        lockMesh.SetActive(Activate);
        unlockMesh.SetActive(!Activate);
    }
}
