using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Movement")]
public class Seek : NavMeshMovement
{
    [UnityEngine.Serialization.FormerlySerializedAs("target")]
    public SharedGameObject m_Target;

    [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
    public SharedVector3 m_TargetPosition;

    public override void OnStart()
    {
        base.OnStart();

        SetDestination(Target());
    }

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
            return TaskStatus.Success;

        SetDestination(Target());

        return TaskStatus.Running;
    }

    private Vector3 Target()
    {
        if (m_Target.Value != null)
            return m_Target.Value.transform.position;

        return m_TargetPosition.Value;
    }

    public override void OnReset()
    {
        base.OnReset();
        m_Target = null;
        m_TargetPosition = Vector3.zero;
    }
}