using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1Manager : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    private FadingEffect _fadingEffect;
    private SpawnSystem _spawnSystem;
    private KeepInRange _keepInRange;
    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        _keepInRange = _serviceLocator.GetService<KeepInRange>();
        _fadingEffect = _serviceLocator.GetService<FadingEffect>();
        _spawnSystem = _serviceLocator.GetService<SpawnSystem>();
        _keepInRange.OnMaxDistanceReached += Respawn;
    }
    private void Respawn()
    {
        _fadingEffect.FadeIn();
        _spawnSystem.SpawnAtLastCheckPoint();
        _fadingEffect.FadeOut();
        _keepInRange.ResetValues();
    }
    private void OnDisable()
    {
        _keepInRange.OnMaxDistanceReached -= Respawn;
    }
}
