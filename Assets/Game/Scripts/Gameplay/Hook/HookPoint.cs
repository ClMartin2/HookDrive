using UnityEngine;

public class HookPoint : MonoBehaviour
{
    [SerializeField] private GameObject unlockMesh;
    [SerializeField] private GameObject lockMesh;

    private void Start()
    {
        Unlock();
    }

    public void Lock()
    {
        ActivateLockMesh(true);
    }

    public void Unlock()
    {
        ActivateLockMesh(false);
    }

    private void ActivateLockMesh(bool Activate)
    {
        lockMesh.SetActive(Activate);
        unlockMesh.SetActive(!Activate);
    }
}
