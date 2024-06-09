using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    CinemachineTargetGroup _targetGroup;
    ServiceLocator _serviceLocator;
    [SerializeField] float _weight = 0.5f;
    [SerializeField] float _radius = 1.0f;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService<TargetGroup>(this, true);
    }
    private void Start()
    {
        _targetGroup.GetComponent<CinemachineTargetGroup>();
        _targetGroup.Targets.Add(new CinemachineTargetGroup.Target { 
            Object = _serviceLocator.GetService<Cat>().gameObject.transform,
            Weight = _weight,
            Radius = _radius});
        _targetGroup.Targets.Add(new CinemachineTargetGroup.Target { 
            Object = _serviceLocator.GetService<Ghost>().gameObject.transform,
            Weight = _weight,
            Radius = _radius});
    }
}
