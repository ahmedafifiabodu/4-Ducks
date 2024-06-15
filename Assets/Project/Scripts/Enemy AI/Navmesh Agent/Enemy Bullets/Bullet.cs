using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _autoDestoryByTime = 5f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private int _damage = 10;

    public event Action OnHit; // Event triggered when the bullet hits a target

    protected Transform _target;

    private Rigidbody _rigidbody;
    private float _timeEnabled;

    internal float AutoDestoryByTime { set => _autoDestoryByTime = value; }

    internal float Speed { get => _speed; set => _speed = value; }

    internal int Damage { set => _damage = value; }

    internal Rigidbody Rigidbody { get => _rigidbody; }
    internal (Vector3, ForceMode) RigidbodyAddForce { set => _rigidbody.AddForce(value.Item1, value.Item2); }

    protected virtual void Awake()
    {
        if (!TryGetComponent(out _rigidbody))
            Logging.LogError("Failed to get Rigidbody component");
    }

    protected virtual void OnEnable() => _timeEnabled = Time.time;

    protected virtual void Update()
    {
        float currentTime = Time.time;
        if (currentTime - _timeEnabled >= _autoDestoryByTime)
            Disable();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            OnHit?.Invoke();
            damageable.TakeDamage(_damage);
        }

        Disable();
    }

    internal virtual void Spawn(Vector3 _forward, int _damage, Transform _target)
    {
        this._damage = _damage;
        this._target = _target;

        _rigidbody.AddForce(_forward * _speed, ForceMode.VelocityChange);
    }

    protected void Disable()
    {
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}