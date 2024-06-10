using Unity.Cinemachine;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    CinemachineTargetGroup _targetGroup;
    ServiceLocator _serviceLocator;
    [SerializeField] float _weight = 1.0f;
    [SerializeField] float _radius = 0.5f;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }
    private void Start()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        if (_targetGroup != null)
        {
            _targetGroup.Targets.Add(new CinemachineTargetGroup.Target
            {
                Object = _serviceLocator.GetService<Cat>().GetTransform(),
                Weight = _weight,
                Radius = _radius
            });
            _targetGroup.Targets.Add(new CinemachineTargetGroup.Target
            {
                Object = _serviceLocator.GetService<Ghost>().GetTransform(),
                Weight = _weight,
                Radius = _radius
            });
        }
        else
        { Logging.Log("haai i am null"); }
    }
}
