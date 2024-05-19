using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    private Camera _camera;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService<CameraInstance>(this, true);
        _camera = GetComponent<Camera>();
    }
    public float ScreenHieghttoWorld()
    {
        Vector3 topRightCorner = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.nearClipPlane));
        Vector3 bottomLeftCorner = _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));

        return topRightCorner.y - bottomLeftCorner.y;
    }
}
