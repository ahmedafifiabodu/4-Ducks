using UnityEngine;
using UnityEngine.Events;

public class HealthCrystal : Crystal
{
    [SerializeField] private float _healAmount;

    [SerializeField] internal static UnityEvent<float, ObjectType> OnHealthCrystalCollected = new();

    public float HPAmount => _healAmount;

    protected override void Collect(ObjectType _playerType)
    {
        base.Collect(_playerType);
        OnHealthCrystalCollected?.Invoke(_healAmount, _playerType);
    }
}