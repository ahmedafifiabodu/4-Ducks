using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    private ServiceLocator _serviceLocator;

    [SerializeField] private bool _isCombined;
    public bool IsCombined => _isCombined;

    [SerializeField] private EnergySystem _catEenergySystem;
    [SerializeField] private EnergySystem _ghostEnergySystem;
    [SerializeField] private EnergySystem _combinedEnergySystem;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }
    private void OnEnable()
    {
        EnergyCrystal.OnEnergyCrystalCollected.AddListener(GainEnergy);
    }

    private void GainEnergy(float _energyAmount, PlayerType _playerType)
    {
        if(_isCombined)
        {
            _combinedEnergySystem.GainEnergy(_energyAmount);
        }
        else 
        {
            //switch(_playerType)
            //{
            //    case { Cat: Cat cat }:
            //        L
            //        break;
            //}
        }
    }
    private void OnDisable()
    {
        EnergyCrystal.OnEnergyCrystalCollected.RemoveListener(GainEnergy);
    }
}
