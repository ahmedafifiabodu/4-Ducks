using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class NavmeshEnemyMovment : MonoBehaviour
{
    [Header("Enemy Movment")]
    [SerializeField] private EnemyLineChecker _lineOfSightChecker;

    [SerializeField] private float _updateSpeed = 0.1f;

    private EnemyStates _defaultState;

    private float _idleLocaitonRadius = 4f;
    private float _idleMovingSpeedMultiplier = 0.5f;

    private int _wayPointIndex = 0;

    private NavMeshAgent _agent;
    private Animator _animator;

    internal NavMeshTriangulation _navMeshTriangulation;
    private Vector3[] _waypoints = new Vector3[4];

    private EnemyStates _state;
    private Cat _catTarget;
    private AgentLinkMover _agentLinkMover;
    public StateChangeEvent _onStateChange;

    public const string Walk = "Walk";
    public const string Jump = "Jump";
    public const string Idle = "Idle";

    private Coroutine _followTargetCoroutine;

    public delegate void StateChangeEvent(EnemyStates _oldState, EnemyStates _newState);

    internal EnemyLineChecker LineOfSightChecker { get => _lineOfSightChecker; set => _lineOfSightChecker = value; }

    internal EnemyStates State
    { get => _state; set { _onStateChange?.Invoke(_state, value); _state = value; } }

    internal EnemyStates DefaultState
    { get => _state; set { _onStateChange?.Invoke(_defaultState, value); _defaultState = value; } }

    internal float UpdateSpeed { private get => _updateSpeed; set => _updateSpeed = value; }

    internal float IdleLocationRadius { get => _idleLocaitonRadius; set => _idleLocaitonRadius = value; }

    internal float IdleMovingSpeedMultiplier { get => _idleMovingSpeedMultiplier; set => _idleMovingSpeedMultiplier = value; }

    internal Vector3[] WayPointsIndex { get => _waypoints; set => _waypoints = value; }

    private void OnEnable()
    {
        CheckForNull();

        _onStateChange += HandleStateChange;
        _agentLinkMover.OnLinkStart += HandleLinkStart;
        _agentLinkMover.OnLinkEnd += HandleLinkEnd;

        _lineOfSightChecker.OnGainSight += (cat) => State = EnemyStates.Chase;
        _lineOfSightChecker.OnLoseSight += (cat) => State = EnemyStates.Patrol;
    }

    private void OnDisable()
    {
        _onStateChange -= HandleStateChange;
        _agentLinkMover.OnLinkStart -= HandleLinkStart;
        _agentLinkMover.OnLinkEnd -= HandleLinkEnd;

        _lineOfSightChecker.OnGainSight -= (cat) => State = EnemyStates.Chase;
        _lineOfSightChecker.OnLoseSight -= (cat) => State = EnemyStates.Patrol;

        _state = _defaultState;
    }

    private void Update()
    {
        bool isMoving = _agent.velocity.magnitude > 0;
        _animator.SetBool(Walk, isMoving);
        _animator.SetBool(Idle, !isMoving);
    }

    internal void Spawn()
    {
        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (NavMesh.SamplePosition(_navMeshTriangulation.vertices[Random.Range(0, _navMeshTriangulation.vertices.Length)], out NavMeshHit _hit, 2f, _agent.areaMask))
                _waypoints[i] = _hit.position;
            else
                Logging.LogError("Failed to sample position");
        }
        _onStateChange?.Invoke(EnemyStates.Spawn, _defaultState);
    }

    #region Event States

    private System.Collections.IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new(_updateSpeed);

        _agent.speed *= _idleMovingSpeedMultiplier;

        while (true)
        {
            if (_agent.enabled && _agent.isOnNavMesh && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                Vector3 _point = Random.insideUnitSphere * _idleLocaitonRadius;

                if (NavMesh.SamplePosition(_agent.transform.position + new Vector3(_point.x, 0, _point.z), out NavMeshHit hit, 2f, _agent.areaMask))
                    _agent.SetDestination(hit.position);
            }

            yield return wait;
        }
    }

    private System.Collections.IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new(_updateSpeed);

        yield return new WaitUntil(() => _agent.enabled && _agent.isOnNavMesh && _waypoints.Length > 0);
        _agent.SetDestination(_waypoints[_wayPointIndex]);

        while (true)
        {
            if (_agent.isOnNavMesh && _agent.enabled && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _wayPointIndex = (_wayPointIndex + 1) % _waypoints.Length;
                _agent.SetDestination(_waypoints[_wayPointIndex]);
            }

            yield return wait;
        }
    }

    private System.Collections.IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new(_updateSpeed);
        while (enabled)
        {
            CheckForNull();
            _agent.SetDestination(_catTarget.transform.position);

            yield return wait;
        }
    }

    #endregion Event States

    #region Event Handlers

    private void HandleStateChange(EnemyStates _oldState, EnemyStates _newState)
    {
        if (_oldState == _newState) return;

        if (_followTargetCoroutine != null)
            StopCoroutine(_followTargetCoroutine);

        if (_oldState == EnemyStates.Idle)
        {
            _agent.speed /= _idleMovingSpeedMultiplier;
        }

        switch (_newState)
        {
            case EnemyStates.Idle:
                _followTargetCoroutine = StartCoroutine(DoIdleMotion());
                break;

            case EnemyStates.Patrol:
                _followTargetCoroutine = StartCoroutine(DoPatrolMotion());
                break;

            case EnemyStates.Chase:
                _followTargetCoroutine = StartCoroutine(FollowTarget());
                break;
        }
    }

    private void HandleLinkStart() => _animator.SetTrigger(Jump);

    private void HandleLinkEnd()
    {
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, true);
    }

    #endregion Event Handlers

    private void CheckForNull()
    {
        _agent = _agent != null ? _agent : GetComponent<NavMeshAgent>();
        _animator = _animator != null ? _animator : GetComponent<Animator>();
        _agentLinkMover = _agentLinkMover != null ? _agentLinkMover : GetComponent<AgentLinkMover>();
        _catTarget = _catTarget != null ? _catTarget : ServiceLocator.Instance.GetService<Cat>();
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(_waypoints[i], 0.25f);

            if (i + 1 < _waypoints.Length)
                Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
            else
                Gizmos.DrawLine(_waypoints[i], _waypoints[0]);
        }
    }
}