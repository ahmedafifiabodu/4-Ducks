using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.FilePathAttribute;
public class SpawnSystem : MonoBehaviour , IDataPersistence
{
    // List of all CheckPoints in the scene
    [SerializeField] private List<CheckPoint> _checkPoints;
    [SerializeField] CheckPoint _startcheckPoint;
    [SerializeField] int _playerOffset;
    [SerializeField] private UnityEvent _onLastCheckPointReached;

    private ServiceLocator _serviceLocator;
    private FadingEffect _fadingEffect;
    private KeepInRange _keepInRange;
    private LevelVirtualCameras _levelVirtualCameras;

    private Dictionary<string, CheckPoint> _checkpointsDictionary = new();
    private CheckPoint _lastcheckPointReached;
    private Transform _catTransform;
    private Transform _ghostTransform;

    public UnityEvent OnLastCheckPointReached => _onLastCheckPointReached;
    internal List<CheckPoint> CheckPoints => _checkPoints;
    internal CheckPoint LastcheckPointReached { get { return _lastcheckPointReached; } set { _lastcheckPointReached = value; } }
    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService<SpawnSystem>(this, false);
    }
    private void Start() 
    {
        _keepInRange = _serviceLocator.GetService<KeepInRange>();
        _fadingEffect = _serviceLocator.GetService<FadingEffect>();

        _startcheckPoint = _checkPoints[0]; 
        _catTransform = ServiceLocator.Instance.GetService<Cat>().GetTransform();
        _ghostTransform = ServiceLocator.Instance.GetService<Ghost>().GetTransform();
        _levelVirtualCameras = ServiceLocator.Instance.GetService<LevelVirtualCameras>();
        AddCheckPointsToDictionary();
    }
    private void AddCheckPointsToDictionary()
    {
        foreach (CheckPoint _cp in CheckPoints)
        {
            _checkpointsDictionary.Add(_cp.CheckPointId, _cp);
        }
    }
    private void OnEnable()
    {
       CheckPoint._onCheckPointPassed.AddListener(UpdateLastCheckPoint);
        _keepInRange.OnMaxDistanceReached += Respawn;
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
            _keepInRange.ChangeMaxDistance(checkPoint.AreaMaxDistance);
        }
    } 
    public void SpawnAtLastCheckPoint() => SpawnAtCheckPoint(_lastcheckPointReached);
    public void SpawnAtCheckPoint(CheckPoint _checkPoint)
    {
        _catTransform.position = _checkPoint.transform.position + new Vector3(_playerOffset, 0f, 0f);
        _catTransform.rotation = _checkPoint.transform.rotation;

        _ghostTransform.position = _checkPoint.transform.position - new Vector3(_playerOffset, 0f, 0f);
        _ghostTransform.rotation = _checkPoint.transform.rotation;

        _levelVirtualCameras.CloseAllCamera();
        _keepInRange.ChangeMaxDistance(_checkPoint.AreaMaxDistance);
        _levelVirtualCameras.OpenCamera(_checkPoint.CamKey);
    }
    public void SpawnAtCheckPoint(string _checkPointID)
    {
        if(_checkpointsDictionary.ContainsKey(_checkPointID))
            SpawnAtCheckPoint(_checkpointsDictionary[_checkPointID]);
    }
    private void Respawn()
    {
        _fadingEffect.FadeIn();
        SpawnAtLastCheckPoint();
        _fadingEffect.FadeOut();
        _keepInRange.ResetValues();
    }
    public void LoadGame(GameData _gameData)
    {
        if (_gameData._lastCheckPointId != null)
            SpawnAtCheckPoint(_gameData._lastCheckPointId);
        else SpawnAtCheckPoint(_startcheckPoint);
    }
    public void SaveGame(GameData _gameData)
    {
        _gameData._lastCheckPointId = _lastcheckPointReached.CheckPointId;
    }
}
