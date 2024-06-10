using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereCollider;

    private int _damage = 10;
    private float _attackDelay = 0.5f;

    protected internal bool useHomingBullet;
    protected readonly List<IDamageable> _damageables = new();
    protected Coroutine _attackCoroutine;
    protected Transform _transform;
    private WaitForSeconds _wait;

    public event AtackEvent OnAttack;

    public delegate void AtackEvent(IDamageable _target);

    internal float SphereColliderAttackRadius { get => _sphereCollider.radius; set => _sphereCollider.radius = value; }
    internal int Damage { get => _damage; set => _damage = value; }
    internal float AttackDelay { get => _attackDelay; set => _attackDelay = value; }

    protected virtual void Start()
    {
        _wait = new WaitForSeconds(_attackDelay);
        _transform = transform;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageables.Add(damageable);
            _attackCoroutine ??= StartCoroutine(Attack());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageables.Remove(damageable);
            if (_damageables.Count == 0)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }

    protected virtual System.Collections.IEnumerator Attack()
    {
        yield return _wait;

        IDamageable _closestDamageable = null;
        float _closestDistance = float.MaxValue;

        while (_damageables.Count > 0)
        {
            foreach (var damageable in _damageables)
            {
                var distance = Vector3.Distance(_transform.position, damageable.GetTransform().position);
                if (distance < _closestDistance)
                {
                    _closestDistance = distance;
                    _closestDamageable = damageable;
                }
            }

            if (_closestDamageable != null)
            {
                InvokeOnAttack(_closestDamageable);
                _closestDamageable.TakeDamage(Damage);
            }

            _closestDamageable = null;
            _closestDistance = float.MaxValue;

            yield return _wait;

            _damageables.RemoveAll(DisableDamageable);
        }

        _attackCoroutine = null;
    }

    protected bool DisableDamageable(IDamageable damageable) => damageable != null && !damageable.GetTransform().gameObject.activeSelf;

    protected void InvokeOnAttack(IDamageable target) => OnAttack?.Invoke(target);
}