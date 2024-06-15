using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.FilePathAttribute;

public class SpawnSystem : MonoBehaviour
{
    // List of all CheckPoints in the scene
    [SerializeField] private List<CheckPoint> _checkPoints;
    [SerializeField] CheckPoint _startcheckPoint;
   // [SerializeField] GameObject _playersRoot;
    [SerializeField] int _playerOffset;

    [SerializeField] private UnityEvent _onLastCheckPointReached;

    private LevelVirtualCameras _levelVirtualCameras;
    private CheckPoint _lastcheckPointReached;
    private Transform _catTransform;
    private Transform _ghostTransform;
    public UnityEvent OnLastCheckPointReached => _onLastCheckPointReached;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<SpawnSystem>(this, false);
    }
    private void Start() 
    {
        _startcheckPoint = _checkPoints[0]; 
        _catTransform = ServiceLocator.Instance.GetService<Cat>().GetTransform();
        _ghostTransform = ServiceLocator.Instance.GetService<Ghost>().GetTransform();
        _levelVirtualCameras = ServiceLocator.Instance.GetService<LevelVirtualCameras>();
    }
    private void OnEnable()
    {
        CheckPoint._onCheckPointPassed.AddListener(UpdateLastCheckPoint);
        _lastcheckPointReached = _startcheckPoint;
    }
    private void OnDisable() => CheckPoint._onCheckPointPassed.RemoveListener(UpdateLastCheckPoint);
    private void UpdateLastCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPoint.IsPassed)
        {
            _lastcheckPointReached = checkPoint;
            // When The last CheckPoint Of the level Reached
            if (_lastcheckPointReached.gameObject == _checkPoints[_checkPoints.Count - 1].gameObject)
                _onLastCheckPointReached?.Invoke();
            checkPoint.IsPassed = true;
        }
    } 
    public void SpawnAtLastCheckPoint() => SpawnAtCheckPoint(_lastcheckPointReached);
    public void SpawnAtStart() => SpawnAtCheckPoint(_startcheckPoint);
    private void SpawnAtCheckPoint(CheckPoint _checkPoint)
    {
        _catTransform.position = _checkPoint.transform.position + new Vector3(_playerOffset, 0f, 0f);
        _catTransform.rotation = _checkPoint.transform.rotation;

        _ghostTransform.position = _checkPoint.transform.position - new Vector3(_playerOffset, 0f, 0f);
        _ghostTransform.rotation = _checkPoint.transform.rotation;

        _levelVirtualCameras.CloseAllCamera();
        _levelVirtualCameras.OpenCamera(_checkPoint.CamKey);

        //Root based logic 

        //foreach (Transform _player in _playersRoot.transform)
        //{
        //    _player.transform.rotation = Quaternion.identity;
        //    _player.transform.position = new Vector3(_playerOffset, 0, 0);
        //    _playerOffset *= -1;
        //}
        //_playerOffset *= -1;

        //_playersRoot.transform.position = _checkPoint.position;
        //_playersRoot.transform.rotation = _checkPoint.rotation;
    }
}
