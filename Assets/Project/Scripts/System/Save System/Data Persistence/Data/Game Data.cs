using UnityEngine;

[System.Serializable]
public class GameData
{
    public long _lastUpdated;

    public int _deathCount;
    public Vector3 _playerPosition;
    public SerializableDictionary<string, bool> _coinsCollected;
    public SerializableDictionary<string, bool> _crystalsCollected;
    public SerializableDictionary<string, CheckPoint> _checkPointPassed;

    public GameData()
    {
        _deathCount = 0;
        _playerPosition = Vector3.zero;
        _coinsCollected = new SerializableDictionary<string, bool>();
        _crystalsCollected = new SerializableDictionary<string, bool>();
    }

    public int PercentageComplete()
    {
        int _totalcoinsCollected = 0;

        foreach (bool coinCollected in _coinsCollected.Values)
        {
            if (coinCollected)
                _totalcoinsCollected++;
        }

        int _precentageComplete = -1;

        if (_coinsCollected.Count > 0)
        {
            _precentageComplete = (_totalcoinsCollected * 100 / _coinsCollected.Count);
        }

        return _precentageComplete;
    }
}