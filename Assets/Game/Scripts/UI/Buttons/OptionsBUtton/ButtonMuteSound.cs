using UnityEngine;

public class ButtonMuteSound : OptionButton
{
    [SerializeField] private AudioSource audioSource;

    private bool mute;

    protected override void OnClick()
    {
        base.OnClick();

        mute = !mute;
        audioSource.mute = mute;
    }
}
