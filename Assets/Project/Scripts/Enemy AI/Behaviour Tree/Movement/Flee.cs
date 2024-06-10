using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Movement")]
public class Flee : NavMeshMovement
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The agent has fleed when the magnitude is greater than this value")]
    [UnityEngine.Serialization.FormerlySerializedAs("fleedDistance")]
    public SharedFloat m_FleedDistance = 20;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance to look ahead when fleeing")]
    [UnityEngine.Serialization.FormerlySerializedAs("lookAheadDistance")]
    public SharedFloat m_LookAheadDistance = 5;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is fleeing from")]
    [UnityEngine.Serialization.FormerlySerializedAs("target")]
    public SharedGameObject m_Target;

    private bool m_HasMoved;

    public override void OnStart()
    {
        base.OnStart();

        m_HasMoved = false;

        SetDestination(Target());
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Magnitude(transform.position - m_Target.Value.transform.position) > m_FleedDistance.Value)
            return TaskStatus.Success;

        if (HasArrived())
        {
            if (!m_HasMoved)
                return TaskStatus.Failure;

            if (!SetDestination(Target()))
                return TaskStatus.Failure;

            m_HasMoved = false;
        }
        else
        {
            var velocityMagnitude = Velocity().sqrMagnitude;

            if (m_HasMoved && velocityMagnitude <= 0f)
                return TaskStatus.Failure;

            m_HasMoved = velocityMagnitude > 0f;
        }

        return TaskStatus.Running;
    }

    private Vector3 Target() => transform.position + (transform.position - m_Target.Value.transform.position).normalized * m_LookAheadDistance.Value;

    protected override bool SetDestination(Vector3 destination)
    {
        if (!SamplePosition(ref destination))
            return false;

        return base.SetDestination(destination);
    }

    public override void OnReset()
    {
        base.OnReset();

        m_FleedDistance = 20;
        m_LookAheadDistance = 5;
        m_Target = null;
    }
}