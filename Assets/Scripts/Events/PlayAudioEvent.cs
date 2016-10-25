using UnityEngine;

public class PlayAudioEvent : GameEvent
{

    private AudioClip audio;
    public AudioClip Clip
    {
        get
        {
            return audio;
        }
    }

    public PlayAudioEvent(AudioClip audio)
    {
        this.audio = audio;
    }
}
