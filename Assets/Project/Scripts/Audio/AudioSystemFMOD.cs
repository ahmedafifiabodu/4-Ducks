using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystemFMOD : MonoBehaviour
{
    ServiceLocator serviceLocator;
    AudioSystemFMOD AudioSystem;
    FMODEvents FmodSystemn;

    private EventInstance musicEventInstance;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this,true);

    }

    private void Start()
    {
        serviceLocator = ServiceLocator.Instance;
        AudioSystem = serviceLocator.GetService<AudioSystemFMOD>();

        FmodSystemn = ServiceLocator.Instance.GetService<FMODEvents>();
        InitializeMusic(FmodSystemn.music);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance PlayereventInstance = RuntimeManager.CreateInstance(eventReference);
        return PlayereventInstance;
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        musicEventInstance.start();
        Logging.Log("Test Music");
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float)area);
    }
}
