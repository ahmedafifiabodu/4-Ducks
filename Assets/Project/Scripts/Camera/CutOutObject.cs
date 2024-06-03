using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutOutObject : MonoBehaviour
{
    [SerializeField] Transform _catTransform;
    [SerializeField] Transform _ghostTransform;
    [SerializeField] LayerMask _obstaclesMask;
    private Camera _camera;
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
        RaycastHit[] _hitGhostObjects = Physics.RaycastAll(transform.position, _catOffest, _ghostOffest.magnitude, _obstaclesMask);

        for(int i =0; i< _hitCatObjects.Length; i++)
        {
            Material[] _materials  = _hitCatObjects[i].transform.GetComponent<Renderer>().materials;
            for(int j = 0; j < _materials.Length;j++)
            {
                _materials[j].SetVector("_Player1Position", _cutOutCatPos);
            }
        }
        for (int i = 0; i < _hitGhostObjects.Length; i++)
        {
            Material[] _materials = _hitGhostObjects[i].transform.GetComponent<Renderer>().materials;
            for (int j = 0; j < _materials.Length; j++)
            {
                _materials[j].SetVector("_Player2Position", _cutOutCatPos);
            }
        }
    }
}
