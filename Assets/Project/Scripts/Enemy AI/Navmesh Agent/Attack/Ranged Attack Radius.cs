using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private float _sphereCastRadius = 0.1f;

    private Vector3 _bulletSpawnOffset;
    private LayerMask _layerMask;

    private WaitForSeconds _attackDelayWaitForSeconds;
    private ObjectPool _objectPool;
    private RaycastHit _hit;

    private IDamageable _targetDamageable;
    private Bullet _bullet;

    #region Setters

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

        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();
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
                    InvokeOnAttack(_targetDamageable);
                    _navMeshAgent.enabled = true;
                    break;
                }
            }

            if (_targetDamageable != null)
            {
                if (useHomingBullet)
                    _bullet = _objectPool.GetPooledObject(_bullet.gameObject).GetComponent<HomingBullet>();
                else
                    _bullet = _objectPool.GetPooledObject(_bullet.gameObject).GetComponent<Bullet>();

                _bullet.Damage = Damage;
                _bullet.transform.SetPositionAndRotation(_transform.position + _bulletSpawnOffset, _navMeshAgent.transform.rotation);

                _bullet.Spawn(_navMeshAgent.transform.forward, Damage, _targetDamageable.GetTransform());
            }
            else
                _navMeshAgent.enabled = true;

            yield return _attackDelayWaitForSeconds;

            if (_targetDamageable == null || !HasLineOfSightTo(_targetDamageable.GetTransform()))
                _navMeshAgent.enabled = true;

            _damageables.RemoveAll(DisableDamageable);
        }

        _navMeshAgent.enabled = true;
        _attackCoroutine = null;
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