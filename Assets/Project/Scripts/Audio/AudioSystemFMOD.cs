using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioSystemFMOD : MonoBehaviour
{
    private FMODEvents fmodSystem;
    private EventInstance musicEventInstance;

    #region Volume Control
    //Volume Control
    [Header("Volume")]
    [Range(0, 1)]
    [SerializeField] private float musicVolume = 1;

    [Range(0, 1)]
    [SerializeField] private float sfxVolume = 1;

    private Bus musicBus;
    private Bus sfxBus;

    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }
    public FMODEvents FmodSystem => fmodSystem;

    #endregion

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        //masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        fmodSystem = ServiceLocator.Instance.GetService<FMODEvents>();
        InitializeMusic(FmodSystem.Music);
    }

    private void Update()
    {
        musicBus.setVolume(MusicVolume);
        sfxBus.setVolume(SfxVolume);
    }

    //Player Foot Steps
    internal EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance PlayereventInstance = RuntimeManager.CreateInstance(eventReference);
        return PlayereventInstance;
    }
    
    //Player Shooting
    public void PlayerShooting(EventReference catShoot , Vector3 pos)
    {
        RuntimeManager.PlayOneShot(catShoot , pos);
    }

    //Enviroment music
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    internal void SetMusicArea(MusicArea area) => musicEventInstance.setParameterByName("area", (float)area);
}