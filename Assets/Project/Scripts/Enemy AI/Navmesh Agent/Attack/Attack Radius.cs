using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

// Require a SphereCollider component on the game object
[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleEffect; // Particle effect for the attack
    [SerializeField] private SphereCollider _sphereCollider; // SphereCollider for the attack radius

    private int _damage = 10; // Damage dealt by the attack
    private float _attackDelay = 0.5f; // Delay between attacks

    protected readonly List<IDamageable> _damageables = new(); // List of damageable objects in the attack radius
    protected Coroutine _attackCoroutine; // Coroutine for the attack
    protected Transform _transform; // Transform of the game object
    private WaitForSeconds _wait; // WaitForSeconds for the attack delay
    private ObjectPool _objectPool; // Object pool for the particle effect

    // Event for when an attack occurs
    public event AtackEvent OnAttack;

    public delegate void AtackEvent(IDamageable _target); // Delegate for the attack event

    private IDamageable _closestDamageable; // The closest damageable object

    // Properties for the attack radius, damage, and attack delay
    internal float SphereColliderAttackRadius { get => _sphereCollider.radius; set => _sphereCollider.radius = value; }

    internal int Damage { get => _damage; set => _damage = value; }
    internal float AttackDelay { get => _attackDelay; set => _attackDelay = value; }
    internal ObjectPool ObjectPool { get => _objectPool; set => _objectPool = value; }

    // Called when the object is first initialized
    protected virtual void Start()
    {
        // Get the object pool from the service locator
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();
        _wait = new WaitForSeconds(_attackDelay);
        _transform = transform;
    }

    // Called when a Collider enters the trigger
    protected virtual void OnTriggerEnter(Collider other)
    {
        // If the Collider is a damageable object, add it to the list and start the attack coroutine
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageables.Add(damageable);
            _attackCoroutine ??= StartCoroutine(Attack());
        }
    }

    // Called when a Collider exits the trigger
    protected virtual void OnTriggerExit(Collider other)
    {
        // If the Collider is a damageable object, remove it from the list and stop the attack coroutine if the list is empty
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

    // Coroutine for the attack
    protected virtual System.Collections.IEnumerator Attack()
    {
        yield return _wait;

        _closestDamageable = null;
        float _closestDistance = float.MaxValue;

        // While there are damageable objects in the list, find the closest one and attack it
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
                InvokeOnAttack(_closestDamageable);

            _closestDistance = float.MaxValue;

            yield return _wait;

            // Remove any disabled damageable objects from the list
            _damageables.RemoveAll(DisableDamageable);
        }

        _attackCoroutine = null;
    }

    // Check if a damageable object is disabled
    protected bool DisableDamageable(IDamageable damageable) => damageable != null && !damageable.GetTransform().gameObject.activeSelf;

    // Invoke the attack event
    protected void InvokeOnAttack(IDamageable target) => OnAttack?.Invoke(target);

    // Called when the attack animation is completed
    internal virtual void OnAttackAnimationCompleted()
    {
        if (_closestDamageable != null)
        {
            // Deal damage to the closest damageable object
            _closestDamageable.TakeDamage(Damage);

            // If there is a particle effect, play it at the position of the damageable object
            if (_particleEffect != null)
            {
                GameObject _particleEffectGameObject = _objectPool.GetPooledObject(_particleEffect.gameObject);

                _particleEffectGameObject.transform.position = _closestDamageable.GetTransform().position;

                ParticleSystem particleInstance = _particleEffectGameObject.GetComponent<ParticleSystem>();

                particleInstance.Play();

                // Disable the particle effect after it has finished playing
                DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
            }

            _closestDamageable = null;
        }
    }
}