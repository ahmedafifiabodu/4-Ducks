using UnityEngine;
using UnityEngine.Rendering;

public class CutOutObject : MonoBehaviour
{
    [SerializeField] Transform _catTransform;
    [SerializeField] Transform _ghostTransform;
    [SerializeField] LayerMask _obstaclesMask;
    private Camera _camera;

    private Material[] _cutoutMat;
    private bool _needResetPos1;
    private bool _needResetPos2;
    RaycastHit[] __hitObjects;
    private void Start()
    {
        _camera = ServiceLocator.Instance.GetService<CameraInstance>().Camera;
    }
    private void Update()
    {
        Vector2 _cutOutCatPos = _camera.WorldToViewportPoint(_catTransform.position);
        Vector2 _cutOutGhostPos = _camera.WorldToViewportPoint(_ghostTransform.position);

        Vector3 _catOffest = _catTransform.position - transform.position;
        Vector3 _ghostOffest = _ghostTransform.position - transform.position;

       
        RaycastHit[] _hitCatObjects = Physics.RaycastAll(transform.position, _catOffest, _catOffest.magnitude, _obstaclesMask);
        RaycastHit[] _hitGhostObjects = Physics.RaycastAll(transform.position, _ghostOffest, _ghostOffest.magnitude, _obstaclesMask);

        if(_hitCatObjects.Length > 0)
        {
            if(!_needResetPos1) _needResetPos1 = true;
            SetMaterialPosition(_hitCatObjects, "_Player1Position", _cutOutCatPos);
        }
        if (_hitGhostObjects.Length > 0)
        {
            if (!_needResetPos1) _needResetPos2 = true;
            SetMaterialPosition(_hitGhostObjects, "_Player2Position", _cutOutGhostPos);
        }
        // the point is _hit always saves the objects of ghost only try to use events to invoke it.
        if (_needResetPos1)
        {
            ResetMaterialsPosition("_Player1Position");
            _needResetPos1 = false;
        }
        if (_needResetPos2)
        {
            ResetMaterialsPosition("_Player2Position");
            _needResetPos2 = false;
        }
    }
    private void SetMaterialPosition(RaycastHit[] _hitObjects, string _matParamenter ,Vector2 _cutPos)
    {

        __hitObjects = _hitObjects;
        for (int i = 0; i < _hitObjects.Length; ++i)
        {
            _cutoutMat = _hitObjects[i].transform.GetComponent<Renderer>().materials;
            for (int j = 0; j < _cutoutMat.Length; ++j)
            {
                _cutoutMat[j].SetVector(_matParamenter, _cutPos);
            }
        }
    }
    private void ResetMaterialsPosition(string _matParamenter)
    {
        for (int i = 0; i < __hitObjects.Length; ++i)
        {
            _cutoutMat = __hitObjects[i].transform.GetComponent<Renderer>().materials;
            for (int j = 0; j < _cutoutMat.Length; ++j)
            {
                _cutoutMat[j].SetVector(_matParamenter, Vector3.zero);
            }
        }
    }
}
