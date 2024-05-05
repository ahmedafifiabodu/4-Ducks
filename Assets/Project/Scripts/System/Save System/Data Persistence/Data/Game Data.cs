using UnityEngine;

[System.Serializable]
public class GameData
{
    public int _move;
    public int _run;
    public int _jump;

    public long _lastUpdated;

    public int _deathCount;
    public Vector3 _playerPosition;
    public SerializableDictionary<string, bool> _coinsCollected;

    public GameData()
    {
        _move = 0;
        _run = 0;
        _jump = 0;

        _deathCount = 0;
        _playerPosition = Vector3.zero;
        _coinsCollected = new SerializableDictionary<string, bool>();
    }

    public int PercentageComplete()
    {
        int _totalcoinsCollected = 0;

        foreach (bool coinCollected in _coinsCollected.Values)
        {
            if (coinCollected)
            {
                _totalcoinsCollected++;
            }
        }

        int _precentageComplete = -1;

        if (_coinsCollected.Count > 0)
        {
            _precentageComplete = (_totalcoinsCollected * 100 / _coinsCollected.Count);
        }

        return _precentageComplete;
    }
}