using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject;

    [SerializeField] private AttackRadius _attackRadius;
    [SerializeField] private NavmeshEnemyMovment _navmeshEnemyMovment;

    [Header("Enemy Components")]
    [SerializeField] private Animator _animator;

    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Coroutine _lookCoroutine;

    internal NavMeshAgent NavMeshAgent => _navMeshAgent;
    internal AttackRadius AttackRadius => _attackRadius;
    internal NavmeshEnemyMovment NavmeshEnemyMovment => _navmeshEnemyMovment;

    private void Awake()
    {
        _attackRadius.useHomingBullet = _enemyScriptableObject.AttackingConfiguration.IsRanged;
        _attackRadius.OnAttack += OnAttack;
    }

    private void OnDisable() => _navMeshAgent.enabled = false;

    private void OnEnable() => _enemyScriptableObject.SetupEnemy(this);

    private void OnAttack(IDamageable _target)
    {
        _animator.SetTrigger(GameConstant.EnemyAnimation.ATTACK_TRIGGER);

        if (_lookCoroutine != null)
            StopCoroutine(_lookCoroutine);

        _lookCoroutine = StartCoroutine(LookAt(_target.GetTransform()));
    }

    private System.Collections.IEnumerator LookAt(Transform target)
    {
        Quaternion _lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float _time = 0;

        while (_time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _time);
            _time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = _lookRotation;
    }

    public Transform GetTransform() => transform;
}