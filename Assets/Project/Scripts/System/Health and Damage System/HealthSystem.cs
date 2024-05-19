using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour ,IDamagable
{
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDeath;

    [SerializeField] private float _healthMax;
    private float _health;
    public float Health => _health;
    public float HealthMax => _healthMax;
    public float HealthPrecentage => (_health / _healthMax);

    private void Awake()
    {
        _health = _healthMax;
    }
    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount;
        if( _health <= 0 ) 
        {
            _health = 0;
            OnDeath?.Invoke();
        }
        OnHealthChanged?.Invoke();
    }
    public void Heal(float healAmount)
    {
        _health += healAmount;
        if (_health >= _healthMax) _health = _healthMax;
        OnHealthChanged?.Invoke();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
