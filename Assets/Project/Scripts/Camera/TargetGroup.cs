using Unity.Cinemachine;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    [SerializeField] private float _weight = 1.0f;
    [SerializeField] private float _radius = 0.5f;

    private CinemachineTargetGroup _targetGroup;
    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }

    private void Start()
    {
        if (TryGetComponent(out _targetGroup))
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
            Logging.Log("haai i am null");
    }
}