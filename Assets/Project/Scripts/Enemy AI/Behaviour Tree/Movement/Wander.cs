using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Wander : NavMeshMovement
{
    [UnityEngine.Serialization.FormerlySerializedAs("minWanderDistance")]
    public SharedFloat m_MinWanderDistance = 20;

    [UnityEngine.Serialization.FormerlySerializedAs("maxWanderDistance")]
    public SharedFloat m_MaxWanderDistance = 20;

    public SharedFloat m_MaxWanderDegrees = 5;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum length of time that the agent should pause at each destination")]
    [UnityEngine.Serialization.FormerlySerializedAs("minPauseDuration")]
    public SharedFloat m_MinPauseDuration = 0;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
    [UnityEngine.Serialization.FormerlySerializedAs("maxPauseDuration")]
    public SharedFloat m_MaxPauseDuration = 0;

    [UnityEngine.Serialization.FormerlySerializedAs("targetRetries")]
    public SharedInt m_TargetRetries = 1;

    private float m_PauseTime;
    private float m_DestinationReachTime;

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            if (m_MaxPauseDuration.Value > 0)
            {
                if (m_DestinationReachTime == -1)
                {
                    m_DestinationReachTime = Time.time;
                    m_PauseTime = Random.Range(m_MinPauseDuration.Value, m_MaxPauseDuration.Value);
                }
                else if (m_DestinationReachTime + m_PauseTime <= Time.time)
                {
                    if (TrySetTarget())
                        m_DestinationReachTime = -1;
                }
            }
            else
                TrySetTarget();
        }
        return TaskStatus.Running;
    }

    private bool TrySetTarget()
    {
        var direction = transform.forward;
        var attempts = m_TargetRetries.Value;
        Vector3 destination;

        while (attempts > 0)
        {
            direction = Quaternion.Euler(0, Random.Range(-m_MaxWanderDegrees.Value, m_MaxWanderDegrees.Value), 0) * direction;
            destination = transform.position + direction.normalized * Random.Range(m_MinWanderDistance.Value, m_MaxWanderDistance.Value);
            if (SamplePosition(ref destination))
            {
                SetDestination(destination);
                return true;
            }
            attempts--;
        }
        return false;
    }

    public override void OnReset()
    {
        m_MinWanderDistance = 20;
        m_MaxWanderDistance = 20;
        m_MaxWanderDegrees = 5;
        m_MinPauseDuration = 0;
        m_MaxPauseDuration = 0;
        m_TargetRetries = 1;
    }
}