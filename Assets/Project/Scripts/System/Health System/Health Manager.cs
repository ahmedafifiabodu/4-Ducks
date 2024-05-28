using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    private ServiceLocator _serviceLocator;

    [SerializeField] private HealthSystem _catHealthSystem;
    [SerializeField] private HealthSystem _combinedHealthSystem;
    [SerializeField] private bool _isCombined;
    
    public HealthSystem CatHealthSystem => _catHealthSystem;
    public HealthSystem CombinedHealthSystem => _combinedHealthSystem;
    public bool IsCombined => _isCombined;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }
    private void OnEnable()
    {
        HealthCrystal.OnHealthCrystalCollected.AddListener(Heal);
    }
    public void Heal(float _healAmount, PlayerType _playerType)
    {
        if(_isCombined)
        {
            _combinedHealthSystem.Heal(_healAmount);
        }else
        {
            if (_playerType.IsPlayerCat)
            {
                _catHealthSystem.Heal(_healAmount);
            }
        }
    }
    public void TakeDamage(float _damageAmount, PlayerType _playerType)
    {
        if (_isCombined)
        {
            _combinedHealthSystem.TakeDamage(_damageAmount);
        }
        else
        {
            if (_playerType.IsPlayerCat)
            {
                _catHealthSystem.TakeDamage(_damageAmount);
            }
        }
    }
}
