using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : Interactable, IDataPersistence
{
    [SerializeField] private string _checkPointId;
    private bool isPassed;

    internal static UnityEvent<Transform> _onCheckPointPassed = new();
    protected override void Interact(ObjectType _playerType)
    {
        base.Interact(_playerType);
        isPassed = true;
        _onCheckPointPassed?.Invoke(transform);
    }
    [ContextMenu("Generate guid for ID")]
    private void GenerateID() => _checkPointId = System.Guid.NewGuid().ToString();
    public void LoadGame(GameData _gameData)
    { 
        _gameData._checkPointPassed.TryGetValue(_checkPointId, out isPassed);
    }
    public void SaveGame(GameData _gameData)
    {
        if (_gameData._checkPointPassed.ContainsKey(_checkPointId))
            _gameData._checkPointPassed[_checkPointId] = isPassed;
        else
            _gameData._checkPointPassed.Add(_checkPointId, isPassed);
    }
}
