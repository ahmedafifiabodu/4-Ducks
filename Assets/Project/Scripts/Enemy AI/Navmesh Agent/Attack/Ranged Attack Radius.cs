using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private float _sphereCastRadius = 0.1f;

    private bool _useHomingBullet;

    private Vector3 _bulletSpawnOffset;
    private LayerMask _layerMask;

    private WaitForSeconds _attackDelayWaitForSeconds;
    private RaycastHit _hit;

    private IDamageable _targetDamageable;
    private IDamageable _storedTargetDamageable;
    private Bullet _bullet;

    #region Setters

    internal bool UseHomingBullet
    { get => _useHomingBullet; set { _useHomingBullet = value; } }

    internal Bullet Bullet
    {
        set { _bullet = value; }
    }

    internal Vector3 BulletSpawnOffset
    {
        set { _bulletSpawnOffset = value; }
    }

    internal LayerMask LineOfSightLayers
    {
        set { _layerMask = value; }
    }

    #endregion Setters

    protected override void Start()
    {
        base.Start();

        ObjectPool = ServiceLocator.Instance.GetService<ObjectPool>();
        _attackDelayWaitForSeconds = new WaitForSeconds(AttackDelay);
    }

    protected override System.Collections.IEnumerator Attack()
    {
        yield return _attackDelayWaitForSeconds;

        while (_damageables.Count > 0)
        {
            _targetDamageable = null;

            for (int i = 0; i < _damageables.Count; i++)
            {
                if (HasLineOfSightTo(_damageables[i].GetTransform()))
                {
                    _targetDamageable = _damageables[i];
                    _storedTargetDamageable = _targetDamageable; // Store the target here
                    InvokeOnAttack(_targetDamageable);
                    _navMeshAgent.enabled = true;
                    break;
                }
            }


            yield return _attackDelayWaitForSeconds;

            if (_targetDamageable == null || !HasLineOfSightTo(_targetDamageable.GetTransform()))
                _navMeshAgent.enabled = true;

            _damageables.RemoveAll(DisableDamageable);
        }

        _navMeshAgent.enabled = true;
        _attackCoroutine = null;
    }

    internal override void OnAttackAnimationCompleted()
    {
        // Check if there's a stored target and if the bullet is ready
        if (_storedTargetDamageable != null && _bullet != null)
        {
            if (ObjectPool == null)
            {
                Logging.LogError("ObjectPool is null. Please assign the ObjectPool in the inspector.");
                return;
            }

            if (ObjectPool.GetPooledObject(_bullet.gameObject) == null)
            {
                Logging.LogError("Failed to get a pooled object. Is the pool size configured correctly?");
                return;
            }

            if (_useHomingBullet)
                _bullet = ObjectPool.GetPooledObject(_bullet.gameObject).GetComponent<HomingBullet>();
            else
                _bullet = ObjectPool.GetPooledObject(_bullet.gameObject).GetComponent<Bullet>();

            _bullet.Damage = Damage;
            _bullet.transform.SetPositionAndRotation(transform.position + _bulletSpawnOffset, _navMeshAgent.transform.rotation);

            _bullet.Spawn(_navMeshAgent.transform.forward, Damage, _storedTargetDamageable.GetTransform());

            // Reset the stored target after shooting
            _storedTargetDamageable = null;
        }
    }

    private bool HasLineOfSightTo(Transform _target)
    {
        Vector3 origin = _transform.position + _bulletSpawnOffset;
        Vector3 direction = (_target.position + _bulletSpawnOffset - origin).normalized;

        if (Physics.SphereCast(origin, _sphereCastRadius, direction, out _hit, SphereColliderAttackRadius * 2, _layerMask))
            if (_hit.transform.TryGetComponent(out IDamageable damageable))
                return damageable.GetTransform() == _target;

        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (_attackCoroutine == null)
            _navMeshAgent.enabled = true;
    }
}