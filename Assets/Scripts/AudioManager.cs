using UnityEngine;
using strange.extensions.mediation.impl;

public class AudioManager : View, IAudioManager
{

    [Inject]
    public IEventManager EventManager { get; set; }

    public AudioSource source;

    // Use this for initialization
    protected override void Start()
    {
        EventManager.AddListener<PlayAudioEvent>(HandlePlayAudio);
    }

    protected override void OnDestroy()
    {
        EventManager.RemoveListener<PlayAudioEvent>(HandlePlayAudio);
    }

    private void HandlePlayAudio(PlayAudioEvent e)
    {
        if (e.Clip != null)
        {
            source.clip = e.Clip;
            source.Play();
        }
    }
}
