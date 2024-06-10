using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField] private List<CheckPoint> _checkPoints;
    [SerializeField] Transform _startcheckPoint;
    [SerializeField] Transform _lastcheckPointReached;
    [SerializeField] GameObject _playersRoot;
    [SerializeField] int _playerOffset;

    [SerializeField] private UnityEvent _onLastCheckPontReached;
    public UnityEvent OnLastCheckPontReached => _onLastCheckPontReached;

    private void Start() => _startcheckPoint = _checkPoints[0].transform;
    private void OnEnable() => CheckPoint._onCheckPointPassed.AddListener(UpdateLastCheckPoint);
    private void OnDisable() => CheckPoint._onCheckPointPassed.RemoveListener(UpdateLastCheckPoint);
    private void UpdateLastCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPoint.IsPassed)
        {
            _lastcheckPointReached = checkPoint.transform;
            if (_lastcheckPointReached.gameObject == _checkPoints[_checkPoints.Count - 1].gameObject)
                _onLastCheckPontReached?.Invoke();
        }
    } 
    public void SpawnAtLastCheckPoint() => SpawnAtCheckPoint(_lastcheckPointReached);
    public void SpawnAtStart() => SpawnAtCheckPoint(_startcheckPoint);
    private void SpawnAtCheckPoint(Transform _checkPoint)
    {
        foreach (Transform _player in _playersRoot.transform)
        {
            _player.transform.rotation = Quaternion.identity;
            _player.transform.position = new Vector3(_playerOffset, 0, 0);
            _playerOffset *= -1;
        }
        _playerOffset *= -1;

        _playersRoot.transform.position = _checkPoint.position;
        _playersRoot.transform.rotation = _checkPoint.rotation;
    }
}
