using UnityEngine;
using UnityEngine.AI;

// Enemy class inherits from MonoBehaviour
public class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject; // Scriptable object for enemy configuration

    [SerializeField] private AttackRadius _attackRadius; // Attack radius for the enemy
    [SerializeField] private NavmeshEnemyMovment _navmeshEnemyMovment; // Movement for the enemy

    [Header("Enemy Components")]
    [SerializeField] private Animator _animator; // Animator for the enemy

    [SerializeField] private NavMeshAgent _navMeshAgent; // NavMeshAgent for the enemy

    private Coroutine _lookCoroutine; // Coroutine for looking at the target

    // Properties to access the components
    internal NavMeshAgent NavMeshAgent => _navMeshAgent;

    internal AttackRadius AttackRadius => _attackRadius;
    internal NavmeshEnemyMovment NavmeshEnemyMovment => _navmeshEnemyMovment;

    // Called when the object is first initialized
    private void Awake()
    {
        // Set up the attack radius
        _attackRadius.useHomingBullet = _enemyScriptableObject.AttackingConfiguration.IsRanged;
        _attackRadius.OnAttack += OnAttack;
    }

    // Called when the object is disabled
    private void OnDisable() => _navMeshAgent.enabled = false;

    // Called when the object is enabled
    private void OnEnable() => _enemyScriptableObject.SetupEnemy(this);

    // Called when the enemy attacks
    private void OnAttack(IDamageable _target)
    {
        // Play the attack animation
        _animator.SetTrigger(GameConstant.EnemyAnimation.ATTACK_TRIGGER);

        // Stop the look coroutine if it is running
        if (_lookCoroutine != null)
            StopCoroutine(_lookCoroutine);

        // Start the look coroutine
        _lookCoroutine = StartCoroutine(LookAt(_target.GetTransform()));
    }

    // Coroutine to make the enemy look at the target
    private System.Collections.IEnumerator LookAt(Transform target)
    {
        Quaternion _lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float _time = 0;

        // Rotate the enemy to look at the target
        while (_time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _time);
            _time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = _lookRotation;
    }

    // Method to get the Transform component of the enemy
    public Transform GetTransform() => transform;
}