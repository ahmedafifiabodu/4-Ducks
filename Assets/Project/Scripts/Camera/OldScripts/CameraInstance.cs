using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private Camera _camera;
    public Camera Camera => _camera;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        ServiceLocator.Instance.RegisterService<CameraInstance>(this, true);
    }
}
