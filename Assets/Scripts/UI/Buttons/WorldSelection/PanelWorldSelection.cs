using UnityEngine;

public class PanelWorldSelection : MonoBehaviour
{
    [SerializeField] private BtnWorldSelection btnWorldSelection;

    public WorldData worldData { get; private set; }

    private void Awake()
    {
        worldData = btnWorldSelection.worldData;
    }

    public void Lock()
    {
        btnWorldSelection.Lock();
    }

    public void UnLock()
    {
        btnWorldSelection.Unlock();
    }
}
