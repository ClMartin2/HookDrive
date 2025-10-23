using UnityEngine;

public class HookPoint : MonoBehaviour
{
    [SerializeField] private GameObject unlockMesh;
    [SerializeField] private GameObject lockMesh;
    [SerializeField] private GameObject onMesh;

    public bool isLocked { get; private set; }

    private void Start()
    {
        Unlock();
        UnAttached();
    }

    public void Lock()
    {
        isLocked = true;
        onMesh.SetActive(true);
        SoundManager.Instance.PlaySoundSFX(SoundManager.LockHook.audioClip, SoundManager.LockHook.volume);
    }

    public void Unlock()
    {
        isLocked = false;
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
