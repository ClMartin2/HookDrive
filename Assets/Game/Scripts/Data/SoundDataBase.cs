using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataBase", menuName = "Scriptable Objects/SoundDataBase")]
public class SoundDataBase : ScriptableObject
{
    public AudioClipToVolume carThrottle;
    public AudioClipToVolume carThrottleLoop;
    public AudioClipToVolume hookStart;
    public AudioClipToVolume lockHook;
    public AudioClipToVolume winLevel;
    public AudioClipToVolume cheering;
    public AudioClipToVolume confetti;
    public AudioClipToVolume winStar;
    public AudioClipToVolume landing;
    public AudioClipToVolume clickBtn;
    public AudioClipToVolume hookOff;
}

[Serializable]
public class AudioClipToVolume
{
    public AudioClip audioClip;
    public float volume = 1f;
    public bool pitchVarition = true;
}
