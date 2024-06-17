using System;
using UnityEngine;
using UnityEngine.AI;

// The Enemy class manages enemy behavior, including movement, attacking, and animations.
public class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject; // Reference to the scriptable object containing enemy configuration settings

    [SerializeField] private AttackRadius _attackRadius; // Component that defines the radius within which the enemy can attack
    [SerializeField] private NavmeshEnemyMovment _navmeshEnemyMovment; // Component that handles enemy movement using the NavMesh system

    [Header("Enemy Components")]
    [SerializeField] private Animator _animator; // Animator component for controlling enemy animations

    [SerializeField] private NavMeshAgent _navMeshAgent; // NavMeshAgent component for pathfinding and movement

    private Coroutine _lookCoroutine; // Coroutine used for smoothly rotating the enemy to face its target

    public event Action OnDestroyed; // Event triggered when the enemy is destroyed

    // Properties for accessing the enemy's components
    internal NavMeshAgent NavMeshAgent => _navMeshAgent;

    internal AttackRadius AttackRadius => _attackRadius;
    internal NavmeshEnemyMovment NavmeshEnemyMovment => _navmeshEnemyMovment;

    // Awake is called when the script instance is being loaded
    private void Awake() => AttackRadius.OnAttack += OnAttack; // Subscribe to the OnAttack event

    // OnDisable is called when the object becomes disabled or inactive
    private void OnDisable() => _navMeshAgent.enabled = false; // Disable the NavMeshAgent

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable() => _enemyScriptableObject.SetupEnemy(this); // Use the scriptable object to configure the enemy

    // Called when the enemy attacks
    private void OnAttack(IDamageable _target)
    {
        _animator.SetTrigger(GameConstant.EnemyAnimation.ATTACK_TRIGGER); // Trigger the attack animation

        if (_lookCoroutine != null)
            StopCoroutine(_lookCoroutine); // Stop the look coroutine if it's already running

        _lookCoroutine = StartCoroutine(LookAt(_target.GetTransform())); // Start a new look coroutine to face the target
    }

    // Coroutine to smoothly rotate the enemy to face its target
    private System.Collections.IEnumerator LookAt(Transform target)
    {
        Quaternion _lookRotation = Quaternion.LookRotation(target.position - transform.position); // Calculate the rotation needed to look at the target
        float _time = 0;

        // Smoothly rotate towards the target over time
        while (_time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _time);
            _time += Time.deltaTime * 2; // Increment time based on deltaTime
            yield return null;
        }

        transform.rotation = _lookRotation; // Ensure the enemy is exactly facing the target at the end
    }

    // Method to get the Transform component of the enemy
    public Transform GetTransform() => transform;

    // Method to be called when the enemy is destroyed
    public void DestroyEnemy() => OnDestroyed?.Invoke();
}