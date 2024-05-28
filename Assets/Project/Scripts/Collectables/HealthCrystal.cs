using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthCrystal : Crystal
{
    public static UnityEvent<float, PlayerType> OnHealthCrystalCollected = new UnityEvent<float, PlayerType>();

    [SerializeField] private float _healAmount;
    public float HPAmount => _healAmount;
    public override void Ability()
    {
        // Still donot know 
    }
    protected override void Collect(PlayerType _playerType)
    {
        base.Collect(_playerType);
        OnHealthCrystalCollected?.Invoke(_healAmount, _playerType);
    }
}
