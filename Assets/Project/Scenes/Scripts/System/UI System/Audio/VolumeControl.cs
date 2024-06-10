using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VolumeType
{
    Music,
    SFX,
}

public class VolumeControl : MonoBehaviour
{

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;
    private AudioSystemFMOD _audioSystem;

    private void Awake()
    {
        volumeSlider = this.GetComponent<Slider>();
    }
    private void Start()
    {
        _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();

    }

    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.Music:
                volumeSlider.value = _audioSystem.MusicVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = _audioSystem.SfxVolume;
                break;
            default:
                Logging.Log("Invalid Type !");
                break;
        }

    }
    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.Music:
                _audioSystem.MusicVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                _audioSystem.SfxVolume = volumeSlider.value;
                break;
            default:
                Logging.Log("Invalid Type !");
                break;
        }
    }

}
