using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("EventListener")]
    public PlayAudioEventSO fxEvent;
    public PlayAudioEventSO bgmEvent;

    public AudioSource BGMSource;
    public AudioSource FXSourece;

    private void OnEnable()
    {
        fxEvent.OnEventRaised += PlayFX;
        bgmEvent.OnEventRaised += PlayBGM;
    }

    private void OnDisable()
    {
        fxEvent.OnEventRaised -= PlayFX;
        bgmEvent.OnEventRaised -= PlayBGM;
    }

    private void PlayBGM(AudioClip clip)
    {
        BGMSource.clip = clip;
        if (!BGMSource.isPlaying)
        {
            BGMSource.Play();
        }
        else
        {
            BGMSource.Stop();
            BGMSource.Play();
        }
    }

    private void PlayFX(AudioClip clip)
    {
        FXSourece.clip = clip;
        FXSourece.Play();
    }
}
