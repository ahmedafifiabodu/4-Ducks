using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Manages spawning and checkpoint logic within the game
public class SpawnSystem : MonoBehaviour, IDataPersistence
{
    // Serialized fields for assigning in the Unity Editor
    [SerializeField] private List<CheckPoint> _checkPoints; // List of all CheckPoints in the scene

    [SerializeField] private CheckPoint _startcheckPoint; // The starting checkpoint
    [SerializeField] private int _playerOffset; // Offset distance for player spawn position
    [SerializeField] private UnityEvent _onLastCheckPointReached; // Event triggered when the last checkpoint is reached

    // Private fields for internal logic
    private ServiceLocator _serviceLocator; // Reference to the ServiceLocator

    private FadingEffect _fadingEffect; // Reference to the FadingEffect component
    private KeepInRange _keepInRange; // Reference to the KeepInRange component
    private LevelVirtualCameras _levelVirtualCameras; // Reference to the LevelVirtualCameras component

    private Dictionary<string, CheckPoint> _checkpointsDictionary = new(); // Dictionary for quick checkpoint lookup
    private CheckPoint _lastcheckPointReached; // The last checkpoint that was reached
    private Transform _catTransform; // Transform of the cat character
    private Transform _ghostTransform; // Transform of the ghost character

    // Public properties for accessing private fields
    public UnityEvent OnLastCheckPointReached => _onLastCheckPointReached;

    internal List<CheckPoint> CheckPoints => _checkPoints;

    internal CheckPoint LastcheckPointReached
    {
        get { return _lastcheckPointReached; }
        set { _lastcheckPointReached = value; }
    }

    private void Awake()
    {
        // Initialize service references
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, false);
    }

    private void Start()
    {
        // Get services from the ServiceLocator
        _keepInRange = _serviceLocator.GetService<KeepInRange>();
        _fadingEffect = _serviceLocator.GetService<FadingEffect>();
        _catTransform = _serviceLocator.GetService<Cat>().GetTransform();
        _ghostTransform = _serviceLocator.GetService<Ghost>().GetTransform();
        _levelVirtualCameras = _serviceLocator.GetService<LevelVirtualCameras>();

        // Subscribe to events
        _keepInRange.OnMaxDistanceReached += Respawn;
        _startcheckPoint = _checkPoints[0];

        // Populate the checkpoints dictionary for quick lookup
        AddCheckPointsToDictionary();
    }

    // Populates the _checkpointsDictionary for efficient checkpoint lookup
    private void AddCheckPointsToDictionary()
    {
        foreach (CheckPoint _cp in CheckPoints)
            _checkpointsDictionary.Add(_cp.CheckPointId, _cp);
    }

    private void OnEnable()
    {
        // Subscribe to checkpoint passed event
        CheckPoint._onCheckPointPassed.AddListener(UpdateLastCheckPoint);
        _lastcheckPointReached = _startcheckPoint;
    }

    private void OnDisable() => CheckPoint._onCheckPointPassed.RemoveListener(UpdateLastCheckPoint);

    // Updates the last checkpoint reached and triggers events if necessary
    private void UpdateLastCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPoint.IsPassed)
        {
            _lastcheckPointReached = checkPoint;

            // Trigger event if the last checkpoint of the level is reached
            if (_lastcheckPointReached.gameObject == _checkPoints[^1].gameObject)
                _onLastCheckPointReached?.Invoke();

            checkPoint.IsPassed = true;
            _keepInRange.ChangeMaxDistance(checkPoint.AreaMaxDistance);
        }
    }

    // Spawns the player at the last checkpoint reached
    public void SpawnAtLastCheckPoint() => SpawnAtCheckPoint(_lastcheckPointReached);

    // Spawns the player at a specific checkpoint
    public void SpawnAtCheckPoint(CheckPoint _checkPoint)
    {
        // Set player and ghost positions and rotations based on checkpoint
        _catTransform.SetPositionAndRotation(_checkPoint.transform.position + new Vector3(_playerOffset, 0f, 0f), _checkPoint.transform.rotation);
        _ghostTransform.SetPositionAndRotation(_checkPoint.transform.position - new Vector3(_playerOffset, 0f, 0f), _checkPoint.transform.rotation);

        // Manage virtual cameras for the current checkpoint
        _levelVirtualCameras.CloseAllCamera();
        _levelVirtualCameras.OpenCamera(_checkPoint.CamKey);

        // Update the maximum distance for the KeepInRange component
        _keepInRange.ChangeMaxDistance(_checkPoint.AreaMaxDistance);
    }

    // Spawns the player at a checkpoint specified by ID
    public void SpawnAtCheckPoint(string _checkPointID)
    {
        if (_checkpointsDictionary.ContainsKey(_checkPointID))
            SpawnAtCheckPoint(_checkpointsDictionary[_checkPointID]);
    }

    // Handles player respawn logic
    private void Respawn()
    {
        _fadingEffect.FadeIn();

        SpawnAtLastCheckPoint();

        _fadingEffect.FadeOut();
        _keepInRange.ResetValues();
    }

    // Loads game state from saved data
    public void LoadGame(GameData _gameData)
    {
        if (_gameData._lastCheckPointId != null)
            SpawnAtCheckPoint(_gameData._lastCheckPointId);
        else SpawnAtCheckPoint(_startcheckPoint);
    }

    // Saves game state to data
    public void SaveGame(GameData _gameData) => _gameData._lastCheckPointId = _lastcheckPointReached.CheckPointId;
}