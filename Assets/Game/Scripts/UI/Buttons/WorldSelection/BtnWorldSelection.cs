using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnWorldSelection : CustomButton
{
    [SerializeField] public WorldData worldData;
    [SerializeField] private Sprite lockImage;
    [SerializeField] private Sprite unLockImage;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI txt;
    
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.LoadWorld?.Invoke(worldData);
        SoundManager.Instance.PlaySoundSFX(SoundManager.ClickBtn.audioClip, SoundManager.ClickBtn.volume);
    }

    public void Lock()
    {
        SetButtonVisual(false);
    }

    public void Unlock() {
        SetButtonVisual(true);
    }

    private void SetButtonVisual(bool unLock)
    {
        image.sprite = unLock? unLockImage : lockImage;
        txt.gameObject.SetActive(unLock);
        button.interactable = unLock;
        image.preserveAspect = !unLock;
    }
}
