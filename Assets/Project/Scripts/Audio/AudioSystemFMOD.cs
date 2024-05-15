using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystemFMOD : MonoBehaviour
{
    ServiceLocator serviceLocator;
    AudioSystemFMOD AudioSystem;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this,true);

        serviceLocator = ServiceLocator.Instance;
        AudioSystem = serviceLocator.GetService<AudioSystemFMOD>();
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance PlayereventInstance = RuntimeManager.CreateInstance(eventReference);
        return PlayereventInstance;
    }
}
