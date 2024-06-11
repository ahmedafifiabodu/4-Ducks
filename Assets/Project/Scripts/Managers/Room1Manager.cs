using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1Manager : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.GetService<KeepInRange>().OnMaxDistanceReached += Respawn;
    }
    private void Respawn()
    {
        _serviceLocator.GetService<FadingEffect>().FadeIn();
        _serviceLocator.GetService<SpawnSystem>().SpawnAtLastCheckPoint();
        _serviceLocator.GetService<FadingEffect>().FadeOut();
    }
}
