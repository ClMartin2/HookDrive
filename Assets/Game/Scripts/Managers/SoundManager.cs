using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource music;
    [field: SerializeField] public AudioSource motorLoop { get; private set; }
    [field: SerializeField] public AudioSource throttleAudioSource { get; private set; }

    [SerializeField] private SoundDataBase soundDatabase;

    [Header("Audio Low Pass Filter")]
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private float startCutoffFrequency = 5000;
    [SerializeField] private float muffledCutoffFrequency = 430;
    [SerializeField] private float durationProgressive = 0.3f;

    public static AudioClipToVolume CarThrottle { get { return Instance.soundDatabase.carThrottle; } }
    public static AudioClipToVolume CarThrottleLoop { get { return Instance.soundDatabase.carThrottleLoop; } }
    public static AudioClipToVolume HookStart { get { return Instance.soundDatabase.hookStart; } }
    public static AudioClipToVolume LockHook { get { return Instance.soundDatabase.lockHook; } }
    public static AudioClipToVolume WinLevel { get { return Instance.soundDatabase.winLevel; } }
    public static AudioClipToVolume Cheering { get { return Instance.soundDatabase.cheering; } }
    public static AudioClipToVolume Confetti { get { return Instance.soundDatabase.confetti; } }
    public static AudioClipToVolume WinStar { get { return Instance.soundDatabase.winStar; } }
    public static AudioClipToVolume Landing { get { return Instance.soundDatabase.landing; } }
    public static AudioClipToVolume ClickBtn { get { return Instance.soundDatabase.clickBtn; } }

    public static SoundManager Instance { get; private set; }

    private float actualCutoffFrequency = 0;
    private float timerLerpCutOffFrequency = 0;
    private Coroutine corouTineLerpCutOffFrequency;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnApplicationFocus(bool focus)
    {
        PauseSound(focus);
    }

    private void OnApplicationPause(bool pause)
    {
        PauseSound(pause);
    }

    public void PlaySoundSFX(AudioClip audio, float volumeScale = 1)
    {
        sfx.PlayOneShot(audio, volumeScale);
        sfx.pitch = Random.Range(0.9f, 1.1f);
    }

    public void PlaySoundThrottle(AudioClip audio, bool loop,float volumeScale = 1)
    {
        throttleAudioSource.loop = loop;
        throttleAudioSource.PlayOneShot(audio, volumeScale);
        throttleAudioSource.pitch = Random.Range(0.95f, 1.05f);
    }

    public void StopAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void SetMusicInUI(bool inUI)
    {
        actualCutoffFrequency = audioLowPassFilter.cutoffFrequency;
        timerLerpCutOffFrequency = 0;

        if (corouTineLerpCutOffFrequency != null)
            StopCoroutine(corouTineLerpCutOffFrequency);

        corouTineLerpCutOffFrequency = StartCoroutine(SetProgressiveCutOffFrequency(inUI));
    }

    private IEnumerator SetProgressiveCutOffFrequency(bool inUI)
    {
        float targetCutOffFrequency = inUI ? muffledCutoffFrequency : startCutoffFrequency;

        while (timerLerpCutOffFrequency <= durationProgressive)
        {
            timerLerpCutOffFrequency += Time.deltaTime;

            float delta = timerLerpCutOffFrequency / durationProgressive;
            audioLowPassFilter.cutoffFrequency = Mathf.Lerp(actualCutoffFrequency, targetCutOffFrequency, delta);
            yield return null;
        }

        audioLowPassFilter.cutoffFrequency = targetCutOffFrequency;
        yield return null;
    }

    public void PauseSound(bool pause)
    {
        if (!pause)
        {
            sfx.Pause();
            music.Pause();
            motorLoop.Pause();
            throttleAudioSource.Pause();
        }
        else
        {
            sfx.UnPause();
            music.UnPause();
            motorLoop.UnPause();
            throttleAudioSource.UnPause();
        }
    }
}
