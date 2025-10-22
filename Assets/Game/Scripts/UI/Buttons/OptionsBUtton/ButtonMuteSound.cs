using UnityEngine;
using UnityEngine.Audio;

public class ButtonMuteSound : OptionButton
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string volumeParameter = "VolumeMusic";
    [SerializeField] private float mutedVolume = -10000f;
    [SerializeField] private GameObject mutedFeedback;

    private float unmutedVolume = 0f;
    private bool mute;

    protected override void Awake()
    {
        base.Awake();
        
        audioMixer.GetFloat(volumeParameter, out unmutedVolume);
        mutedFeedback.gameObject.SetActive(false);
    }

    protected override void OnClick()
    {
        base.OnClick();

        mute = !mute;

        audioMixer.SetFloat(volumeParameter, mute ? mutedVolume : unmutedVolume);
        mutedFeedback.gameObject.SetActive(mute);

        if (audioSource != null)
        {
            if (mute)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }
    }

}
