using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private Camera _camera;
    private CinemachineBrain _brain;
    public Camera Camera => _camera;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _brain = GetComponent<CinemachineBrain>();
        ServiceLocator.Instance.RegisterService<CameraInstance>(this, true);
    }
    internal void ChangeCustomBlend(CinemachineBlenderSettings customBlend) => _brain.CustomBlends=customBlend;
}
