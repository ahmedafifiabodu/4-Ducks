using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Crystal : MonoBehaviour, ICollectable
{
    public static UnityEvent<float> OnCrystalCollected = new UnityEvent<float>();

    [SerializeField] private string _crystalId;
    private bool isCollected = false;
    public abstract void Ability();
    public virtual void Collect()
    {
        OnCrystalCollected?.Invoke(GetEnergyAmount());
        gameObject.SetActive(false);
        isCollected = true;
    }
    protected abstract float GetEnergyAmount();

    [ContextMenu("Generate guid for ID")]
    private void GenerateID()
    {
        _crystalId = System.Guid.NewGuid().ToString();
    }

    public void LoadGame(GameData _gameData)
    {
        _gameData._crystalsCollected.TryGetValue(_crystalId, out isCollected);

        if (isCollected)
            gameObject.SetActive(false);
    }

    public void SaveGame(GameData _gameData)
    {
        if (_gameData._crystalsCollected.ContainsKey(_crystalId))
        {
            _gameData._crystalsCollected[_crystalId] = isCollected;
        }
        else
        {
            _gameData._crystalsCollected.Add(_crystalId, isCollected);
        }
    }
}
