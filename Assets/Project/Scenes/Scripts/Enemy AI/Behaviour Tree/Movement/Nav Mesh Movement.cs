using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : Movement
{
    #region Properties

    [UnityEngine.Serialization.FormerlySerializedAs("speed")]
    public SharedFloat m_Speed = 10;

    [UnityEngine.Serialization.FormerlySerializedAs("angularSpeed")]
    public SharedFloat m_AngularSpeed = 120;

    [BehaviorDesigner.Runtime.Tasks.Tooltip(
        "The agent has arrived when the destination is less than the specified amount. " +
        "This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
    [UnityEngine.Serialization.FormerlySerializedAs("arriveDistance")]
    public SharedFloat m_ArriveDistance = 0.2f;

    [UnityEngine.Serialization.FormerlySerializedAs("stopOnTaskEnd")]
    public SharedBool m_StopOnTaskEnd = true;

    [UnityEngine.Serialization.FormerlySerializedAs("updateRotation")]
    public SharedBool m_UpdateRotation = true;

    #endregion Properties

    #region Fields

    protected NavMeshAgent m_NavMeshAgent;
    private bool m_StartUpdateRotation;

    #endregion Fields

    #region Overrides Unity Methods

    public override void OnAwake() => m_NavMeshAgent = GetComponent<NavMeshAgent>();

    public override void OnStart()
    {
        m_NavMeshAgent.speed = m_Speed.Value;
        m_NavMeshAgent.angularSpeed = m_AngularSpeed.Value;
        m_NavMeshAgent.isStopped = false;
        m_StartUpdateRotation = m_NavMeshAgent.updateRotation;
        UpdateRotation(m_UpdateRotation.Value);
    }

    #endregion Overrides Unity Methods

    #region Overrides Methods

    protected override bool SetDestination(Vector3 destination)
    {
        Vector3 directionToTarget = (destination - transform.position).normalized;
        Vector3 finalDestination = destination - directionToTarget * m_ArriveDistance.Value;

        m_NavMeshAgent.isStopped = false;
        return m_NavMeshAgent.SetDestination(finalDestination);
    }

    protected override void UpdateRotation(bool update)
    {
        m_NavMeshAgent.updateRotation = update;
        m_NavMeshAgent.updateUpAxis = update;
    }

    protected override bool HasPath() => m_NavMeshAgent.hasPath && m_NavMeshAgent.remainingDistance > m_ArriveDistance.Value;

    protected override Vector3 Velocity() => m_NavMeshAgent.velocity;

    protected override bool HasArrived()
    {
        float remainingDistance;

        if (m_NavMeshAgent.pathPending)
            remainingDistance = float.PositiveInfinity;
        else
            remainingDistance = m_NavMeshAgent.remainingDistance;

        return remainingDistance <= m_ArriveDistance.Value;
    }

    protected override void Stop()
    {
        UpdateRotation(m_StartUpdateRotation);

        if (m_NavMeshAgent.hasPath)
            m_NavMeshAgent.isStopped = true;
    }

    public override void OnReset()
    {
        m_Speed = 10;
        m_AngularSpeed = 120;
        m_ArriveDistance = 1;
        m_StopOnTaskEnd = true;
    }

    #endregion Overrides Methods

    #region Behavior Designer Methods

    public override void OnEnd()
    {
        if (m_StopOnTaskEnd.Value)
            Stop();
        else
            UpdateRotation(m_StartUpdateRotation);
    }

    public override void OnBehaviorComplete() => Stop();

    #endregion Behavior Designer Methods

    #region Methods

    protected bool SamplePosition(ref Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, m_NavMeshAgent.height * 2, NavMesh.AllAreas))
        {
            position = hit.position;
            return true;
        }
        return false;
    }

    #endregion Methods
}