using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private MusicArea area;

    AudioSystemFMOD _audioSystem;

    private void Start()
    {
        _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioSystem.SetMusicArea(area);
        }
    }
}
