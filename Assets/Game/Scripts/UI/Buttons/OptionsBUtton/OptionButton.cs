using UnityEngine;

[RequireComponent(typeof(AnimScriptScale))]
public class OptionButton : CustomButton
{
    private AnimScriptScale animScriptScale;

    private void OnEnable()
    {
        animScriptScale = GetComponent<AnimScriptScale>();
    }

    protected override void OnClick()
    {
        base.OnClick();
        animScriptScale.Scale();
        SoundManager.Instance.PlaySoundSFX(SoundManager.ClickBtn.audioClip, SoundManager.ClickBtn.volume);
    }
}
