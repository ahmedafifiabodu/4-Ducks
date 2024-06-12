using UnityEngine;
using UnityEngine.Events;

public class KeepInRange : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _dangerDistance;

    private Transform _catTransform;
    private Transform _ghostTransform;
    private ServiceLocator _serviceLocator;
    private bool inDanger;
    private bool maxDistanceReached;

    private UnityAction _onMaxDistanceReached;
    private UnityAction _onDanger;

    internal void MaxDistance(float maxDistance) => _maxDistance = maxDistance;

    internal void DangerDistance(float dangerDistance) => _dangerDistance = dangerDistance;

    internal UnityAction OnMaxDistanceReached
    { set { _onMaxDistanceReached = value; } get { return _onMaxDistanceReached; } }
    internal UnityAction OnDanger { set { _onDanger = value; } get { return _onDanger; } }

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService<KeepInRange>(this, true);
    }

    private void Start()
    {
        _catTransform = _serviceLocator.GetService<Cat>().GetTransform();
        _ghostTransform = _serviceLocator.GetService<Ghost>().GetTransform();
    }

    private void LateUpdate()
    {
        float _currentDistance = Vector3.Distance(_catTransform.position, _ghostTransform.position);
        if (!maxDistanceReached || !inDanger)
        {
            if (_currentDistance >= _maxDistance)
            {
                maxDistanceReached = true;
                _onMaxDistanceReached?.Invoke();
            }
            else if (_currentDistance >= _dangerDistance)
            {
                inDanger = true;
                _onDanger?.Invoke();
            }
        }
        else if (inDanger && _currentDistance < _dangerDistance)
        {
            inDanger = false;
        }
    }

    internal void ResetValues()
    {
        inDanger = false;
        maxDistanceReached = false;
    }
}