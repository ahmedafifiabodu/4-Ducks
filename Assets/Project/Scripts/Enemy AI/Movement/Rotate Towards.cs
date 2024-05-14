using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Movement")]
public class RotateTowards : Action
{
    [UnityEngine.Serialization.FormerlySerializedAs("rotationEpsilon")]
    public SharedFloat m_RotationEpsilon = 0.5f;

    [UnityEngine.Serialization.FormerlySerializedAs("maxLookAtRotationDelta")]
    public SharedFloat m_MaxLookAtRotationDelta = 1;

    [UnityEngine.Serialization.FormerlySerializedAs("onlyY")]
    public SharedBool m_OnlyY;

    [UnityEngine.Serialization.FormerlySerializedAs("target")]
    public SharedGameObject m_Target;

    [UnityEngine.Serialization.FormerlySerializedAs("targetRotation")]
    public SharedVector3 m_TargetRotation;

    public override TaskStatus OnUpdate()
    {
        var rotation = Target();
        if (Quaternion.Angle(transform.rotation, rotation) < m_RotationEpsilon.Value)
            return TaskStatus.Success;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, m_MaxLookAtRotationDelta.Value);
        return TaskStatus.Running;
    }

    private Quaternion Target()
    {
        if (m_Target == null || m_Target.Value == null)
            return Quaternion.Euler(m_TargetRotation.Value);

        var position = m_Target.Value.transform.position - transform.position;

        if (m_OnlyY.Value)
            position.y = 0;

        return Quaternion.LookRotation(position);
    }

    public override void OnReset()
    {
        m_RotationEpsilon = 0.5f;
        m_MaxLookAtRotationDelta = 1f;
        m_OnlyY = false;
        m_Target = null;
        m_TargetRotation = Vector3.zero;
    }
}