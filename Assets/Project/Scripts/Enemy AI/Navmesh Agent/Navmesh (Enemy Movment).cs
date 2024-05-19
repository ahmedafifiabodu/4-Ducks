using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class NavmeshEnemyMovment : MonoBehaviour
{
    [SerializeField] private float _updateSpeed = 0.1f;

    internal float UpdateSpeed { private get => _updateSpeed; set => _updateSpeed = value; }

    private NavMeshAgent _agent;
    private Animator _animator;

    private Cat _catTarget;

    private AgentLinkMover _agentLinkMover;

    public const string Walk = "Walk";
    public const string Jump = "Jump";
    public const string Idle = "Idle";

    private Coroutine _followTargetCoroutine;

    private void Start()
    {
        CheckForNull();

        _agentLinkMover.OnLinkStart += HandleLinkStart;
        _agentLinkMover.OnLinkEnd += HandleLinkEnd;
    }

    internal void StartChasing()
    {
        if (_followTargetCoroutine == null)
            _followTargetCoroutine = StartCoroutine(FollowTarget());
        else
            Logging.LogWarning("Follow target coroutine is already running");
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

    private void HandleLinkStart() => _animator.SetTrigger(Jump);

    private void HandleLinkEnd()
    {
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, true);
    }

    private void Update()
    {
        if (_agent.velocity.magnitude > 0)
        {
            _animator.SetBool(Walk, true);
            _animator.SetBool(Idle, false);
        }
        else
        {
            _animator.SetBool(Walk, false);
            _animator.SetBool(Idle, true);
        }
    }

    private void CheckForNull()
    {
        if (_catTarget == null)
            _catTarget = ServiceLocator.Instance.GetService<Cat>();

        if (_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_agentLinkMover == null)
            _agentLinkMover = GetComponent<AgentLinkMover>();
    }
}