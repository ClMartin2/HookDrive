using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource music;
    [field: SerializeField] public AudioSource carAccelLoop { get; private set; }
    [field: SerializeField] public AudioSource cardIlde { get; private set; }
    [field: SerializeField] public AudioSource hookLoop { get; private set; }
    [field: SerializeField] public AudioSource car { get; private set; }

    [SerializeField] private SoundDataBase soundDatabase;

    [Header("Audio Low Pass Filter")]
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private float startCutoffFrequency = 5000;
    [SerializeField] private float muffledCutoffFrequency = 430;
    [SerializeField] private float durationProgressive = 0.3f;

    public static AudioClipToVolume CarStart { get { return Instance.soundDatabase.carStart; } }
    public static AudioClipToVolume CarAccelLoop { get { return Instance.soundDatabase.carAccelLoop; } }
    public static AudioClipToVolume CarAccelStart { get { return Instance.soundDatabase.carAccelStart; } }
    public static AudioClipToVolume CarDecelerate { get { return Instance.soundDatabase.carDecelerate; } }
    public static AudioClipToVolume CarIdleLoop { get { return Instance.soundDatabase.carIdleLoop; } }
    public static AudioClipToVolume HookStart { get { return Instance.soundDatabase.hookStart; } }
    public static AudioClipToVolume LockHook { get { return Instance.soundDatabase.lockHook; } }
    public static AudioClipToVolume WinLevel { get { return Instance.soundDatabase.winLevel; } }
    public static AudioClipToVolume Cheering { get { return Instance.soundDatabase.cheering; } }
    public static AudioClipToVolume Confetti { get { return Instance.soundDatabase.confetti; } }
    public static AudioClipToVolume WinStar { get { return Instance.soundDatabase.winStar; } }
    public static AudioClipToVolume Landing { get { return Instance.soundDatabase.landing; } }
    public static AudioClipToVolume ClickBtn { get { return Instance.soundDatabase.clickBtn; } }
    public static AudioClipToVolume HookOff { get { return Instance.soundDatabase.hookOff; } }

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

    public void PlaySoundSFX(AudioClip audio, float volumeScale = 1, bool pitchVariation = true)
    {
        sfx.PlayOneShot(audio, volumeScale);
        sfx.pitch = pitchVariation? Random.Range(0.9f, 1.1f) : 1f;
    }

    public void PlaySoundCar(AudioClip audio, float volumeScale = 1, bool pitchVariation = true)
    {
        car.PlayOneShot(audio, volumeScale);
        car.pitch = pitchVariation? Random.Range(0.9f, 1.1f) : 1f;
    }

    public void PlayHookLoop()
    {
        hookLoop.Play();
    }

    public void PlayCarAccelLoop()
    {
        carAccelLoop.Play();
    }

    public void PlayCarIdleLoop()
    {
        cardIlde.Play();
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
            carAccelLoop.Pause();
            cardIlde.Pause();
            hookLoop.Pause();
            car.Pause();
        }
        else
        {
            sfx.UnPause();
            music.UnPause();
            carAccelLoop.UnPause();
            cardIlde.UnPause();
            hookLoop.UnPause();
            car.UnPause();
        }
    }
}
