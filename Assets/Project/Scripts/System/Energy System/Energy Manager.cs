using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    private ServiceLocator _serviceLocator;

    [SerializeField] private bool _isCombined;

    [SerializeField] private EnergySystem _catEenergySystem;
    [SerializeField] private EnergySystem _ghostEnergySystem;
    [SerializeField] private EnergySystem _combinedEnergySystem;

    public bool IsCombined => _isCombined;
    public EnergySystem CatEnergySystem => _catEenergySystem;
    public EnergySystem GhostEnergySystem => _ghostEnergySystem;
    public EnergySystem CombinedEnergySystem => _combinedEnergySystem;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }
    private void OnEnable()
    {
        EnergyCrystal.OnEnergyCrystalCollected.AddListener(GainEnergy);
    }
    public void GainEnergy(float _energyAmount, PlayerType _playerType)
    {
        if(_isCombined)
        {
            _combinedEnergySystem.GainEnergy(_energyAmount);
        }
        else 
        {
            if (_playerType.IsPlayerCat)
            {
                _catEenergySystem.GainEnergy(_energyAmount);
            }
            if(_playerType.IsPlayerGhost)
            {
                _ghostEnergySystem.GainEnergy(_energyAmount);
            }
        }
    }
    public void LoseEnergy(float _energyAmount, PlayerType _playerType)
    {
        if (_isCombined)
        {
            _combinedEnergySystem.LoseEnergy(_energyAmount);
        }
        else
        {
            if (_playerType.IsPlayerCat)
            {
                _catEenergySystem.LoseEnergy(_energyAmount);
            }
            if (_playerType.IsPlayerGhost)
            {
                _ghostEnergySystem.LoseEnergy(_energyAmount);
            }
        }
    }
    public void EditCombinedState(bool isCombined)
    {
        _isCombined = isCombined;
    }
    private void OnDisable()
    {
        EnergyCrystal.OnEnergyCrystalCollected.RemoveListener(GainEnergy);
    }
}
