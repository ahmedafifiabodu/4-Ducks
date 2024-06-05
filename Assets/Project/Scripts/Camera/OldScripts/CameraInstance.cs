using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    [SerializeField] private Camera _camera;
    public Camera Camera => _camera;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService<CameraInstance>(this, true);
      //  _camera = GetComponent<Camera>();
    }
}
