using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSkyBox : MonoBehaviour
{
    [SerializeField] private Material _DaySkyMat;
    [SerializeField] private Material _nightSkyMat;
    public void SwitchToNight()
    {
        RenderSettings.skybox = _nightSkyMat;
    }
    public void SwitchToDay()
    {
        RenderSettings.skybox = _DaySkyMat;
    }
}
