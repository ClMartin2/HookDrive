using UnityEngine;
using UnityEngine.UI;

public class PanelWorldSelection : MonoBehaviour
{
    [SerializeField] private BtnWorldSelection btnWorldSelection;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private Image trophy;
    [SerializeField] private Sprite bronzeTrophy;
    [SerializeField] private Sprite silverTrophy;
    [SerializeField] private Sprite goldTrophy;

    public WorldData worldData { get; private set; }

    private void Awake()
    {
        worldData = btnWorldSelection.worldData;
    }

    public void Lock()
    {
        btnWorldSelection.Lock();
        lockImage.SetActive(true);
        trophy.gameObject.SetActive(false);
    }

    public void UnLock()
    {
        btnWorldSelection.Unlock();
        lockImage.SetActive(false);
        trophy.gameObject.SetActive(true);

        switch (worldData.actualMedal)
        {
            case Medal.none:
                trophy.gameObject.SetActive(false);
                break;
            case Medal.bronze:
                trophy.sprite = bronzeTrophy;
                break;
            case Medal.silver:
                trophy.sprite = silverTrophy;
                break;
            case Medal.gold:
                trophy.sprite = goldTrophy;
                break;
        }

    }
}
