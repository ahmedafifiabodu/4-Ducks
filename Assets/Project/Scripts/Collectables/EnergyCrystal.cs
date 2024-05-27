using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnergyCrystal : Crystal
{
    public static UnityEvent<float, PlayerType> OnEnergyCrystalCollected = new UnityEvent<float, PlayerType>();

    [SerializeField] private float _energyAmount;
    public float EnergyAmount => _energyAmount;
    public override void Ability()
    {
        throw new System.NotImplementedException();
    }
    protected override void Collect(PlayerType _playerType)
    {
        base.Collect(_playerType);
        OnEnergyCrystalCollected?.Invoke(_energyAmount, _playerType);
    }
}
