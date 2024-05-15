using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthSystem 
{
    public UnityEvent _damageEvent;
    public UnityEvent _healEvent;
    public UnityEvent _deathEvent;

    private float _health;
    private float _healthMax;
    public float Health => _health;
    public float HealthPrecentage => (_health / _healthMax);
    public HealthSystem(float healthMax) 
    {
        _healthMax = healthMax;
        _health = healthMax;
    }
    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount;
        if( _health <= 0 ) 
        {
            _health = 0;
            _deathEvent?.Invoke();
        }
        _damageEvent?.Invoke();
    }
    public void Heal(float healAmount)
    {
        _health += healAmount;
        if (_health >= _healthMax) _health = _healthMax;
        _healEvent.Invoke();
    }
}
