using UnityEngine;

public class AudioDefinition : MonoBehaviour
{
    [Header("Broadcast")]
    public PlayAudioEventSO playAudioEvent;

    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if(playOnEnable)
        {
            PlayAudioClip();
        }
    }

    public void PlayAudioClip()
    {
        playAudioEvent.RaiseEvent(audioClip);
    }
}
