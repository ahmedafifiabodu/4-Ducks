using UnityEngine;
using UnityEngine.Events;

public class EnergyCrystal : Crystal
{
    [SerializeField] private float _energyAmount;

    [SerializeField] internal static UnityEvent<float, ObjectType> OnEnergyCrystalCollected = new();

    public float EnergyAmount => _energyAmount;

    protected override void Collect(ObjectType _playerType)
    {
        base.Collect(_playerType);
        OnEnergyCrystalCollected?.Invoke(_energyAmount, _playerType);
    }
}