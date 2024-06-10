using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : Interactable, IDataPersistence
{
    [SerializeField] private string _checkPointId;
    private bool _isPassed;
    public bool IsPassed => _isPassed;

    internal static UnityEvent<CheckPoint> _onCheckPointPassed = new();
    protected override void Interact(ObjectType _playerType)
    {
        base.Interact(_playerType);
        _isPassed = true;
        _onCheckPointPassed?.Invoke(this);
    }
    [ContextMenu("Generate guid for ID")]
    private void GenerateID() => _checkPointId = System.Guid.NewGuid().ToString();
    public void LoadGame(GameData _gameData)
    { 
        _gameData._checkPointPassed.TryGetValue(_checkPointId, out _isPassed);
    }
    public void SaveGame(GameData _gameData)
    {
        if (_gameData._checkPointPassed.ContainsKey(_checkPointId))
            _gameData._checkPointPassed[_checkPointId] = _isPassed;
        else
            _gameData._checkPointPassed.Add(_checkPointId, _isPassed);
    }
}
