using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioSystemFMOD : MonoBehaviour
{
    private FMODEvents FmodSystemn;
    private EventInstance musicEventInstance;

    //Volume Control
    [Header("Volume")]
    [Range(0, 1)]
    [SerializeField] private float musicVolume = 1;

    [Range(0, 1)]
    [SerializeField] private float sfxVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        //masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        FmodSystemn = ServiceLocator.Instance.GetService<FMODEvents>();
        InitializeMusic(FmodSystemn.Music);
    }

    private void Update()
    {
        musicBus.setVolume(MusicVolume);
        sfxBus.setVolume(SfxVolume);
    }

    internal EventInstance CreateEventInstance(EventReference eventReference)
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

    internal void SetMusicArea(MusicArea area) => musicEventInstance.setParameterByName("area", (float)area);
}