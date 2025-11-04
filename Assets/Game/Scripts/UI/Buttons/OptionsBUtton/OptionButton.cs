using UnityEngine;

[RequireComponent(typeof(AnimScriptScale))]
public class OptionButton : CustomButton
{
    protected AnimScriptScale animScriptScale;

    virtual protected void OnEnable()
    {
        animScriptScale = GetComponent<AnimScriptScale>();
    }

    protected override void OnClick()
    {
        base.OnClick();
        animScriptScale.Scale();
        SoundManager.Instance.PlaySoundSFX(SoundManager.ClickBtn.audioClip, SoundManager.ClickBtn.volume, SoundManager.ClickBtn.pitchVarition);
    }
}
